using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UnitSelectionComponent : MonoBehaviour
{
    bool isSelecting = false;
    Vector3 mousePosition1;

    private Ray ray;
    private RaycastHit hitInfo;

    public GameObject selectionCirclePrefab;
    public List<SelectableUnitComponent> selectedObjects = new List<SelectableUnitComponent>();
    public Action OnSelectionRest;

    void Update()
    {
        // If we press the left mouse button, begin selection and remember the location of the mouse
        if( Input.GetMouseButtonDown( 0 ) )
        {
            isSelecting = true;
            mousePosition1 = Input.mousePosition;

            foreach( var selectableObject in FindObjectsOfType<SelectableUnitComponent>() )
            {
                if( selectableObject.selectionCircle != null )
                {
                    Destroy( selectableObject.selectionCircle.gameObject );
                    selectableObject.selectionCircle = null;
                }
            }
        }
        // If we let go of the left mouse button, end selection
        if( Input.GetMouseButtonUp( 0 ) )
        {
            if (OnSelectionRest != null) OnSelectionRest();
            selectedObjects.Clear();
            foreach( var selectableObject in FindObjectsOfType<SelectableUnitComponent>() )
            {
                if( IsWithinSelectionBounds( selectableObject.gameObject ) )
                {
                    selectedObjects.Add( selectableObject );
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine( string.Format( "Selecting [{0}] Units", selectedObjects.Count ) );
            foreach( var selectedObject in selectedObjects )
                sb.AppendLine( "-> " + selectedObject.gameObject.name );
            Debug.Log( sb.ToString() );

            if(selectedObjects.Count == 0)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.transform.tag == "Unit")
                    {
                        SelectableUnitComponent selectableObject = hitInfo.transform.gameObject.GetComponent<SelectableUnitComponent>();
                        selectedObjects.Add(selectableObject);
                        if (selectableObject.selectionCircle == null)
                        {
                            selectableObject.selectionCircle = Instantiate(selectionCirclePrefab);
                            selectableObject.selectionCircle.transform.SetParent(selectableObject.transform, false);
                            selectableObject.selectionCircle.transform.eulerAngles = new Vector3(90, 0, 0);
                        }
                    }
                }
            }

            isSelecting = false;
        }

        // Highlight all objects within the selection box
        if( isSelecting )
        {
            foreach( var selectableObject in FindObjectsOfType<SelectableUnitComponent>() )
            {
                if( IsWithinSelectionBounds( selectableObject.gameObject ) )
                {
                    if( selectableObject.selectionCircle == null )
                    {
                        selectableObject.selectionCircle = Instantiate( selectionCirclePrefab );
                        selectableObject.selectionCircle.transform.SetParent( selectableObject.transform, false );
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3( 90, 0, 0 );
                    }
                }
                else
                {
                    if( selectableObject.selectionCircle != null )
                    {
                        Destroy( selectableObject.selectionCircle.gameObject );
                        selectableObject.selectionCircle = null;
                    }
                }
            }
        }
    }

    public bool IsWithinSelectionBounds( GameObject gameObject )
    {
        if( !isSelecting )
            return false;

        var camera = Camera.main;
        var viewportBounds = Utils.GetViewportBounds( camera, mousePosition1, Input.mousePosition );
        return viewportBounds.Contains( camera.WorldToViewportPoint( gameObject.transform.position ) );
    }

    void OnGUI()
    {
        if( isSelecting )
        {
            // Create a rect from both mouse positions
            var rect = Utils.GetScreenRect( mousePosition1, Input.mousePosition );
            Utils.DrawScreenRect( rect, new Color( 0.8f, 0.8f, 0.95f, 0.25f ) );
            Utils.DrawScreenRectBorder( rect, 2, new Color( 0.8f, 0.8f, 0.95f ) );
        }
    }
}