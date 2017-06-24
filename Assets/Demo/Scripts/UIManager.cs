using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private Button btnFeed;
    [SerializeField]
    private Button btnAddChild;

    private void Awake()
    {
        GetComponent<CanvasScaler>().referenceResolution =
            new Vector2( Screen.width, Screen.height );
        btnAddChild.onClick.AddListener( AddChild );
    }

    private void AddChild()
    {
        var childManager = Installer.GetInstance<ChildManager>();
        //var child = Instantiate( childManager.ChildPrefab,
        //    childManager.GetHead.position + ( -childManager.GetHead.forward ), Quaternion.identity );
        //childManager.AddChild( child.transform );
        childManager.CreateChild();
    }

    private void Feed()
    {

    }
}
