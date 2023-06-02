Hi. If you are using different SRP than URP, delete the URP folder and import relevant package.

Thanks for your purchase. I hope you will find the asset useful.
If you have any problem, please email me: izzynab.publisher@gmail.com

I encourage you to leave a review in the asset store. 
Have a nice day :)

Documentation: https://inabstudios.gitbook.io/easy-outline-shader/



Offline Documentation:

How to use:
Built-in:
There is three ways to use the outline shaders:
Add OnlyOutline shader as second material to your object
Add OutlineDrawer script to your object with skinned mesh renderer or mesh filter. Use OnlyOutline shader in Material field.
Use custom standard materials with outlines: OutlineMetallic and OutlineSpecular. This shaders work same as standard shaders but they do not support every feature that standard materials do. You can easily change your old materials to new ones, all properties should remain unchanged.
Masked Outlines:
In order to use this feature, you need to replace your old materials with new ones: OutlineMaskMetallic or OutlineMaskSpecular. After that you need to add OnlyOutlineWithMask shader to your object either by using OutlineDrawer script or as a second material.
URP:
There is three ways to use the outline shaders:
Add OnlyOutline shader as second material to your object.
Add Renderer Feature to forward renderer asset with OnlyOutline override material and layer mask set to your desired layer. You can easily create new layer for each of your objects that need different outlines.
Use custom standard materials with outlines: OutlineMetallic and OutlineSpecular. This shaders work same as standard shaders. You can easily change your old materials to new ones, all properties will remain unchanged.
Masked Outlines:
To use masked outlines, the only thing you have to do is to add 2 render objects renderer features to your forward renderer data. Both of these render features have to draw over the same layer.
This features have to be in this order: 
Mask material, which uses "OutlineMask" shader
Outline material, which uses "OnlyOutlineWithMask" shader.
In order to make the example forward renderer data work, you must change the layer masks to those from your own layers. You can find the example in URP/EasyOutlineRenderer.
HDRP:
Instead of using custom shaders you can modify and use shader graph version of the outline shader. 
There is two ways to use the outline shaders:
Add OnlyOutline shader or Shader graph version as second material to your object.
Add Custom Pass: DrawRendereCustomPass with OnlyOutline override material and filter set to your desired layer. You can easily create new layer for each of your objects that need different outlines.
Masked Outlines:
To use masked outlines, the only thing you have to do is to add 2DrawRendereCustomPasses. Both of these custom passes have to draw over the same layer.
This passes have to be in this order: 
Mask material, which uses "OutlineMask" shader
Outline material, which uses "OnlyOutlineWithMask" shader.
You can find examples of all of this in example scene in Custom Pass for outlines object.

Masked Outlines Hints:
Mask outlines use stencil buffer!
If you want to create few different objects with masked outlines, you need to set different value for StencilReference from 1 to 255 for each mask and outline material.
Properties:
Disable outline from C# by setting _Enable property to 0, and enable it by setting the property to 1.
Make outline thickness more consistent over a range of mesh distances by using Adaptive Thickness.
There are 3 outline types: 
Normal - based on mesh normals. If your outlines do not look as good as you would like, and you don't want to bake new normals onto your meshes, try calculating normals with smoothing angle set to 180 in Import Settings of your models. 
Position - based on mesh vertex positions.
UV Baked - reads values baked in UV3 channel, if you need to bake to another channel, in SmoothNormalsBaker tool choose UV2 or UV 1. This would require modifying shaders with Amplify Shader Editor.
UV Baking does not support skinned meshes.
Gradient:
Intesity - Scale factor of the color. 
Noise Scale - Scale of the noise used to map ramp onto outline. 
Screen Space - Check if you want noise to be drawed in screen space, otherwise it will use tangent space. 
Flow Speed - Speed of the noise offset change. 
Flow Rotation - Rotation of the noise flow.
Ramp Map:
Create gradients by clicking on the gradient editor. When you are ready to save the texture, set path of the ramp png. Then export the texture. After that you need to manually drag and drop the saved texture onto RampMap field on the left.
If you do not export gradient as a texture, it wont't be saved.

Available Tools:
You can bake mesh smoothed normals to UV channel and then use UV Baked option to render outlines using smooth normals even with low poly meshes. Rembember, it does not work with skeletal meshes.
To do that, use Tools/SmoothNormalsBaker window. 
Please, make sure you selected UV3 channel for baking normals. If you are not able to use UV3 channel, you have to change shader code by yourself with Amplify Shader Code editor.
You could always email me and I can do it for you :)
