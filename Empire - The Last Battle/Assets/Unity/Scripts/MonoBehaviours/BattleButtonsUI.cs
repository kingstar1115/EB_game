using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class BattleButtonsUI : MonoBehaviour 
{
    public string _ShowAnimTrigger;
    public string _HideAnimTrigger;
    public Image _PlayerImage;

    Animator _animator;

    bool _enabled;
    public bool _Enabled
    {
        get { return _enabled; }
        set
        {
            //enable/disable buttons
            _enabled = value;
        }
    }

    public void Init()
    {
        //grab the animator and canvas group 
        _animator = this.GetComponent<Animator>();
    }

    public void Show()
    {
        //do the show animation
        _animator.SetTrigger(_ShowAnimTrigger);
    }

    public void Hide()
    {
		//do the hide animation
		_animator.SetTrigger(_HideAnimTrigger);
    }
}
