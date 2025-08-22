using UnityEngine;
using UnityEngine.InputSystem;

public class RebindMenuManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _movePlayer1Reference;
    [SerializeField] private InputActionReference _specialAbilityPlayer1Reference;
    [SerializeField] private InputActionReference _movePlayer2Reference;
    [SerializeField] private InputActionReference _specialAbilityPlayer2Reference;
    
    private void OnEnable()
    {
        _movePlayer1Reference.action.Disable();
        _specialAbilityPlayer1Reference.action.Disable();
        
        _movePlayer2Reference.action.Disable();
        _specialAbilityPlayer2Reference.action.Disable();
    }

    private void OnDisable()
    {
        _movePlayer1Reference.action.Enable();
        _specialAbilityPlayer1Reference.action.Enable();
        
        _movePlayer2Reference.action.Enable();
        _specialAbilityPlayer2Reference.action.Enable();
    }
}
