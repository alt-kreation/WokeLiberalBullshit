
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

[AddComponentMenu("UI (Canvas)/Invert Maskable", 15)]
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class InvertMaskable : UnityEngine.EventSystems.UIBehaviour, IMaterialModifier
{
    Graphic m_thisImage;

    public bool Invert;

    public Graphic thisImage
    {
        get { return m_thisImage ?? (m_thisImage = GetComponent<Graphic>()); }
    }
    private Material m_MaskMaterial;


    private Material m_UnmaskMaterial;

    public virtual Material GetModifiedMaterial(Material baseMaterial)
    {


        var rootSortCanvas = MaskUtilities.FindRootSortOverrideCanvas(transform);
        var stencilDepth = MaskUtilities.GetStencilDepth(transform, rootSortCanvas);
        if (stencilDepth >= 8)
        {
            Debug.LogWarning("Attempting to use a stencil mask with depth > 8", gameObject);
            return baseMaterial;
        }

        int desiredStencilBit = 1 << stencilDepth;

        // if we are at the first level...
        // we want to destroy what is there
        if (desiredStencilBit == 1)
        {
            var maskMaterial = StencilMaterial.Add(baseMaterial, Invert ? 2 : 1, StencilOp.Replace, CompareFunction.Always, ColorWriteMask.All);
            StencilMaterial.Remove(m_MaskMaterial);
            m_MaskMaterial = maskMaterial;

            var unmaskMaterial = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, 0);
            StencilMaterial.Remove(m_UnmaskMaterial);
            m_UnmaskMaterial = unmaskMaterial;
            thisImage.canvasRenderer.popMaterialCount = 1;
            thisImage.canvasRenderer.SetPopMaterial(m_UnmaskMaterial, 0);

            return m_MaskMaterial;
        }
        //otherwise we need to be a bit smarter and set some read / write masks
        var maskMaterial2 = StencilMaterial.Add(baseMaterial, Invert ? 2 : 1, StencilOp.Replace, CompareFunction.Equal, ColorWriteMask.All, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
        StencilMaterial.Remove(m_MaskMaterial);
        m_MaskMaterial = maskMaterial2;

        thisImage.canvasRenderer.hasPopInstruction = true;
        var unmaskMaterial2 = StencilMaterial.Add(baseMaterial, desiredStencilBit - 1, StencilOp.Replace, CompareFunction.Equal, 0, desiredStencilBit - 1, desiredStencilBit | (desiredStencilBit - 1));
        StencilMaterial.Remove(m_UnmaskMaterial);
        m_UnmaskMaterial = unmaskMaterial2;
        thisImage.canvasRenderer.popMaterialCount = 1;
        thisImage.canvasRenderer.SetPopMaterial(m_UnmaskMaterial, 0);

        return m_MaskMaterial;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (thisImage != null)
        {
            thisImage.canvasRenderer.hasPopInstruction = true;
            thisImage.SetMaterialDirty();

            // Default the thisImage to being the maskable thisImage if its found.

        }

        MaskUtilities.NotifyStencilStateChanged(this);
    }

    protected override void OnDisable()
    {
        // we call base OnDisable first here
        // as we need to have the IsActive return the
        // correct value when we notify the children
        // that the mask state has changed.
        base.OnDisable();
        if (thisImage != null)
        {
            thisImage.SetMaterialDirty();
            thisImage.canvasRenderer.hasPopInstruction = false;
            thisImage.canvasRenderer.popMaterialCount = 0;


        }

        StencilMaterial.Remove(m_MaskMaterial);
        m_MaskMaterial = null;
        StencilMaterial.Remove(m_UnmaskMaterial);
        m_UnmaskMaterial = null;

        MaskUtilities.NotifyStencilStateChanged(this);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (!IsActive())
            return;

        if (thisImage != null)
        {
            // Default the thisImage to being the maskable thisImage if its found.


            thisImage.SetMaterialDirty();
        }

        MaskUtilities.NotifyStencilStateChanged(this);
    }

#endif
}


