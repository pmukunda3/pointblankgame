// Aaron Lanterman, July 1, 2019
// Modified example from https://github.com/Unity-Technologies/PostProcessing/wiki/Writing-Custom-Effects

using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Warning from https://github.com/Unity-Technologies/PostProcessing/wiki/Writing-Custom-Effects 
// Because of how serialization works in Unity, you have to make sure that the file is named 
// after your settings class name or it won't be serialized properly.

// This is the settings class
[Serializable]
[PostProcess(typeof(GPU19ColorMissRepresentationRenderer), PostProcessEvent.AfterStack, "Custom/GPU19ColorMissRepresentation")]
public sealed class GPU19ColorMissRepresentation : PostProcessEffectSettings {
    [Tooltip("Speed of crossfade effect.")]
    public FloatParameter speed = new FloatParameter { value = 1f };
	[Tooltip("Shift Color Control")]
	public FloatParameter shift = new FloatParameter { value = 5.0f };
    [Tooltip("Depth Control")]
    public FloatParameter depth = new FloatParameter { value = 1.0f };
}

public sealed class GPU19ColorMissRepresentationRenderer : PostProcessEffectRenderer<GPU19ColorMissRepresentation> {
    public override DepthTextureMode GetCameraFlags() {
        return DepthTextureMode.DepthNormals;
    }

	public override void Render(PostProcessRenderContext context) {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/GPU19ColorMissRepresentationShader"));
        sheet.properties.SetFloat("_Speed", settings.speed);
		sheet.properties.SetFloat("_Shift", settings.shift);
        sheet.properties.SetFloat("_Depth", settings.depth);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
