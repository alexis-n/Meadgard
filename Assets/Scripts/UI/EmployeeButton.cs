using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeButton : MonoBehaviour
{
    #region /// VARIABLES ///
    [HideInInspector] public Employee employee;
    public Employee.EmployeeType employeeType;
    [SerializeField] private KeyCode key;
    #endregion

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key) && gameObject.activeSelf)
        {
            SelectEmployee();
        }
    }

    public void SelectEmployee()
    {
        if (SelectionManager.instance.selectedObject != employee) SelectionManager.instance.SelectNewObject(employee);
        else if (SelectionManager.instance.selectedObject == employee) CameraManager.instance.ViewTarget(employee.gameObject);
    }
}
