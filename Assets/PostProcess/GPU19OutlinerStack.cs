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
[PostProcess(typeof(GPU19OutlinerStackRenderer), PostProcessEvent.AfterStack, "Custom/GPU19OutlinerStack")]
public sealed class GPU19OutlinerStack : PostProcessEffectSettings {
    [Tooltip("Speed of crossfade effect.")]
    public FloatParameter speed = new FloatParameter { value = 1f };
}

public sealed class GPU19OutlinerStackRenderer : PostProcessEffectRenderer<GPU19OutlinerStack> {
    public override void Render(PostProcessRenderContext context) {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/GPU19OutlinerStackShader"));
        sheet.properties.SetFloat("_Speed", settings.speed);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}
