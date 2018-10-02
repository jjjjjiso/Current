using UnityEngine;

public class FSM_SendMessage : StateMachineBehaviour
{
    public string strOnStateEnter  = string.Empty;
    public string strOnStateUpdate = string.Empty;
    public string strOnStateExit   = string.Empty;
    public string strOnStateMove   = string.Empty;
    public string strOnStateIK     = string.Empty;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!string.IsNullOrEmpty(strOnStateEnter))
        {
            animator.SendMessage(strOnStateEnter, SendMessageOptions.RequireReceiver);
        }
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!string.IsNullOrEmpty(strOnStateUpdate))
        {
            animator.SendMessage(strOnStateUpdate, SendMessageOptions.RequireReceiver);
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!string.IsNullOrEmpty(strOnStateExit))
        {
            animator.SendMessage(strOnStateExit, SendMessageOptions.RequireReceiver);
        }
    }

    //OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!string.IsNullOrEmpty(strOnStateMove))
        {
            animator.SendMessage(strOnStateMove, SendMessageOptions.RequireReceiver);
        }
    }

    //OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK(inverse kinematics) should be implemented here.
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!string.IsNullOrEmpty(strOnStateIK))
        {
            animator.SendMessage(strOnStateIK, SendMessageOptions.RequireReceiver);
        }
    }
}
