using System;
using MagicLeap;
using MagicLeap.DesignToolkit.Actions;
using MagicLeap.LeapBrush;
using UnityEngine;

public class ShapeChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject _cube;

    [SerializeField]
    private GameObject _sphere;

    [SerializeField]
    private GameObject _capsule;

    private ShapeChangerProto.Types.Shape _shape = ShapeChangerProto.Types.Shape.Cube;
    private External3DModel _external3dModel;

    public void SetShape(ShapeChangerProto.Types.Shape shape)
    {
        if (_shape == shape)
        {
            return;
        }

        _shape = shape;

        _cube.SetActive(_shape == ShapeChangerProto.Types.Shape.Cube);
        _sphere.SetActive(_shape == ShapeChangerProto.Types.Shape.Sphere);
        _capsule.SetActive(_shape == ShapeChangerProto.Types.Shape.Capsule);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started");

        _external3dModel = GetComponentInParent<External3DModel>();
        if (_external3dModel.ShapeChangerData != null)
        {
            SetShape(_external3dModel.ShapeChangerData.Shape);
        }
        _external3dModel.OnShapeChangerDataChanged += OnShapeChangerDataChanged;

        _external3dModel.GetComponent<Interactable>().Events.OnGrab.AddListener(OnGrab);
    }

    private void OnShapeChangerDataChanged(External3DModel externalModel, bool changedByCurrentUser)
    {
        if (!changedByCurrentUser)
        {
            // Ignore changes by the current user since they would have already applied to the
            // current object.
            SetShape(_external3dModel.ShapeChangerData.Shape);
        }
    }

    private void OnGrab(Interactor _)
    {
        Debug.Log("Grabbed");

        switch (_shape)
        {
            case ShapeChangerProto.Types.Shape.Cube:
                SetShape(ShapeChangerProto.Types.Shape.Sphere);
                DispatchDataChanged();
                return;
            case ShapeChangerProto.Types.Shape.Sphere:
                SetShape(ShapeChangerProto.Types.Shape.Capsule);
                DispatchDataChanged();
                break;
            case ShapeChangerProto.Types.Shape.Capsule:
                SetShape(ShapeChangerProto.Types.Shape.Cube);
                DispatchDataChanged();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void DispatchDataChanged()
    {
        _external3dModel.SetShapeChangerData(new ShapeChangerProto
        {
            Shape = _shape
        }, true);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
