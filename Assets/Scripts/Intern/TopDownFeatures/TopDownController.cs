using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// An abstract Command class, to implement the Command Design Patern
/// </summary>
public abstract class Command 
{
    //execute the command
    public abstract void Execute();

    //the agent has finished the command
    public abstract bool IsFinished();

    //properly end the command
    public abstract void End();
}

/// <summary>
/// Command implementation, to move the agent to a position
/// </summary>
public class MoveCommand : Command
{

    private TopDownAgent m_agent;
    private Vector3 m_targetPosition;

    /// <summary>
    /// create the command with all the informations : 
    /// </summary>
    public MoveCommand(TopDownAgent agent, Vector3 targetPosition)
    {
        m_agent = agent;
        m_targetPosition = targetPosition;
    }

    public override void Execute()
    {
        m_agent.Behaviour = TopDownAgent.TopDownBehaviour.MOVE;

        m_agent.Move(m_targetPosition);
    }

    /// <summary>
    /// Finished when the agent is near the targeted position
    /// </summary>
    public override bool IsFinished()
    {
        //if the agent is near the targeted position 
        if (Vector3.SqrMagnitude(m_agent.transform.position - m_targetPosition) < 1)
            return true;
        else
            return false;
    }

    public override void End()
    {
        //nothing
    }
}

/// <summary>
/// Command implementation, so that the agent attack the target.
/// </summary>
public class MoveAndAttackCommand : Command
{

    private TopDownAgent m_agent;
    private Target m_target;
    private bool m_isFinished = false;
    //the delay between two update
    private float m_aiDelay = 1;
    //ptr to coroutine to properly stop it
    private IEnumerator m_moveAndAttackCoroutine;

    //create the command with all the informations : 
    public MoveAndAttackCommand(TopDownAgent agent, Target target, float aiDelay = 1)
    {
        m_agent = agent;
        m_target = target;
        m_aiDelay = aiDelay;
    }

    public override void Execute()
    {
        m_agent.Behaviour = TopDownAgent.TopDownBehaviour.ATTACK;

        m_moveAndAttackCoroutine = MoveAndAttackRoutine();
        m_agent.StartCoroutine(m_moveAndAttackCoroutine);
    }

    //finished when the target has been killed, or when the target has escaped
    public override bool IsFinished()
    {
        return m_isFinished;
    }

    IEnumerator MoveAndAttackRoutine()
    {
        while (!m_isFinished)
        {
            if (!m_target.isAlive()) // is the target already dead ? 
            {
                m_isFinished = true;
            }
            //Check if agent can attack
            else if (m_agent.CanAttack(m_target))
            {
                m_agent.StopWalking();
                m_agent.attack(m_target);
            }
            else
            {
                m_agent.Move(m_target.transform.position);
            }

            yield return new WaitForSeconds(m_aiDelay);
        }
    }

    public override void End()
    {
        m_agent.StopCoroutine(m_moveAndAttackCoroutine);
    }
}

/// <summary>
/// The TopDownController verify the player input and create appropriate actions that he will give to its agent.
/// </summary>
public class TopDownController : MonoBehaviour
{
    //small struct to retrieve the information of the entity the mouse has clicked on
    struct MouseTargetInfo
    {
        public Vector3 position;
        public GameObject gameObject;
        public string tag;
    }

    //used to store information on the entity the mouse has clicked on (if any)
    MouseTargetInfo m_mouseTargetInfo;

    //the target this component control. If null during instanciation, try to find a TopDownAgent on this entity
    [SerializeField]
    private TopDownAgent m_agent;

    //the right click target
    [SerializeField]
    private Target m_currentTarget;

    //all potential targets 
    private List<Target> m_potentialTargets;

    //delay between two decisions taken by the ia
    [SerializeField]
    private float m_iaDelay;

    //command list. The commands has to be treaten by the controller one after an other
    private Queue<Command> m_commandList = new Queue<Command>();
    //current treaten command
    private Command m_currentCommand = null;

    public void addPotentialTarget(Target target)
    {
        m_potentialTargets.Add(target);
    }

    public void removePotentialTarget(Target target)
    {
        m_potentialTargets.Remove( target );
    }

    void Start()
    {
        //if m_target isn't set up, try to find a TopDownAgent component on this entity
        if (m_agent == null)
            m_agent = GetComponent<TopDownAgent>();
    }
	
	void Update()
    {
        //update inputs only if this controller is active
        if(m_agent.isControllable)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (checkMouseTarget(out m_mouseTargetInfo)) // first step : rayCast and store information on the mouseTargetInfo
                {
                    Debug.Log( "mouse encounter a target with tag : " + m_mouseTargetInfo.tag.ToString() );

                    if( m_mouseTargetInfo.tag == "Target" )
                    {
                        //store a pointer to the current target
                        m_currentTarget = m_mouseTargetInfo.gameObject.GetComponent<Target>();

                        //launch a coroutine for attack behaviour
                        if( m_currentTarget != null )
                        {
                            //MoveAndAttack();

                            AttachNewCommand(new MoveAndAttackCommand(m_agent, m_currentTarget, 0.5f));
                        }
                    }
                    else
                    {
                        //Move( m_mouseTargetInfo.position );

                        AttachNewCommand(new MoveCommand(m_agent, m_mouseTargetInfo.position));
                    }
                }
            }
        }

        //Replace the current command if it is unasigned, or if the command has finished
        if(m_currentCommand == null || m_currentCommand.IsFinished() )
        {
            //if there is an other command to execute, set the current command and execute it
            if (m_commandList.Count > 0)
            {
                //properly end the current command
                if(m_currentCommand != null)
                    m_currentCommand.End();

                //change the current command
                m_currentCommand = m_commandList.Dequeue();

                //Execute the new current command
                m_currentCommand.Execute();
            }
            //no command are set, return to the idle behaviour
            else
            {
                Idle();
            }
        }
	}

    //add a new command to the command list if shift is hold. Otherwise, clear the command list and directly set the new command as the current command and execute it
    void AttachNewCommand(Command newCommand)
    {
        if (Input.GetKey(KeyCode.LeftShift))
            m_commandList.Enqueue(newCommand);
        else
        {
            //properly end the current command
            if (m_currentCommand != null)
                m_currentCommand.End();

            //clear the command list
            if (m_commandList.Count > 0)
                m_commandList.Clear();

            //set and execute the new current command
            m_currentCommand = newCommand;
            m_currentCommand.Execute();
        }
    }

    //fill the struct passed as parameter with informations concerning the mouse click (click position, click on an entity ? , ...  )
    bool checkMouseTarget(out MouseTargetInfo info)
    {
        Ray selectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(selectionRay, out hitInfo))
        {
            info.gameObject = hitInfo.collider.gameObject;
            info.position = hitInfo.point;
            info.tag = hitInfo.collider.tag;
            return true;
        }
        else
        {
            info.gameObject = null;
            info.position = Vector3.zero;
            info.tag = "none";
            return false;
        }
    }





    //Functions to directly change the behaviour of the agent. 
    //Deprecated since a new command design patern has been implemented

    void ResetBehaviour()
    {
        //stop other tasks 
        StopAllCoroutines();

        //change agent task 
        m_agent.Behaviour = TopDownAgent.TopDownBehaviour.IDLE;

        //stop walking
        m_agent.StopWalking();
    }

    void Move( Vector3 position )
    {
        ResetBehaviour();

        //change agent task 
        m_agent.Behaviour = TopDownAgent.TopDownBehaviour.MOVE;

        //perform the task
        m_agent.Move(position);
    }

    void MoveAndAttack()
    {
        ResetBehaviour();

        //change agent task 
        m_agent.Behaviour = TopDownAgent.TopDownBehaviour.ATTACK;

        //perform the task
        StartCoroutine( MoveAndAttackRoutine() );
    }

    void Idle()
    {
        ResetBehaviour();

        //change agent task
        m_agent.Behaviour = TopDownAgent.TopDownBehaviour.IDLE;

        //perform the task
        // TODO
    }

    IEnumerator MoveAndAttackRoutine()
    {
        while(m_agent.Behaviour == TopDownAgent.TopDownBehaviour.ATTACK)
        {
            if(!m_currentTarget.isAlive()) // is the target already dead ? 
            {
                Idle();
            }
            //Check if agent can attack
            else if( m_agent.CanAttack( m_currentTarget ) )
            {
                m_agent.StopWalking();
                m_agent.attack( m_currentTarget );
            }
            else
            {
                m_agent.Move( m_currentTarget.transform.position );
            }

            yield return new WaitForSeconds( m_iaDelay );
        }
    }


}
