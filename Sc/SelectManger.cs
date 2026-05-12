using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Type
{
    easy,
    normal,
    hard
}

public class SelectManger : MonoBehaviour
{
    public Type type;
    public Dropdown legacyDropdown;

    void Start()
    {
        legacyDropdown.onValueChanged.RemoveAllListeners();
        
        legacyDropdown.onValueChanged.AddListener((index) => 
        {
            switch (index)
            {
                case 0: OnEasySelected(); break;
                case 1: OnNormalSelected(); break;
                case 2: OnHardSelected(); break;
            }
        });
    }

    public void StartGame()
    {
        switch (type)
        {
            case Type.easy: SceneManager.LoadScene(1); break;
            case Type.normal: SceneManager.LoadScene(2); break;
            case Type.hard: SceneManager.LoadScene(3); break;
        }
    }

    private void OnEasySelected()
    {
        type = Type.easy;
    }

    private void OnNormalSelected()
    {
        type = Type.normal;
    }

    private void OnHardSelected()
    {
        type = Type.hard;
    }
}