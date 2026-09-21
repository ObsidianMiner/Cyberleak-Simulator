using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UnityExtensions
{

    //==============
    //    Other
    //==============

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
    public static bool Contains(this Collider collider, Vector3 worldPosition)
    {
        var direction = collider.bounds.center - worldPosition;
        var ray = new Ray(worldPosition, direction);

        var contains = collider.Raycast(ray, out var hit, direction.magnitude);

        return contains;
    }
    public static T GetRandom<T>(this T[] t)
    {
        return t[UnityEngine.Random.Range(0, t.Length)];
    }
    /// <summary>
    /// Scales a Vector3, and returns the output. It does not change the Vector3 itself.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="toMultiply"></param>
    /// <returns></returns>
    public static Vector3 QuickScale(this Vector3 vector3, Vector3 toMultiply) => new Vector3(vector3.x * toMultiply.x, vector3.y * toMultiply.y, vector3.z * toMultiply.z);
    public static Vector2 QuickScale(this Vector2 vector2, Vector2 toMultiply) => new Vector3(vector2.x * toMultiply.x, vector2.y * toMultiply.y);
    public static float Size(this Vector3 vector3) => vector3.x + vector3.y + vector3.z;
    public static Vector3 QuickInverseScale(this Vector3 vector3) => new Vector3(1f / vector3.x, 1f / vector3.y, 1f / vector3.z);

    public static Image GetHandle(this Slider slider) => slider.transform.GetChild(2).GetChild(0).GetComponent<Image>();

    public static GameObject GetRandom(this GameObject[] gameObjects) => gameObjects[UnityEngine.Random.Range(0, gameObjects.Length)];
    public static GameObject GetRandom(this List<GameObject> gameObjects) => gameObjects[UnityEngine.Random.Range(0, gameObjects.Count)];

    public static Vector2 XZ(this Vector3 vector3) => new Vector2(vector3.x, vector3.z);
    public static Vector3 XZ(this Vector2 vector2) => new Vector3(vector2.x, 0, vector2.y);

    public static Color ChangeAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    public static int CountTrue(this bool[] array)
    {
        int count = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i]) count++;
        }
        return count;
    }

    public static string ToSpacedString(this Enum value)
    {
        var str = value.ToString();
        var sb = new StringBuilder();

        for (int i = 0; i < str.Length; i++)
        {
            if (char.IsUpper(str[i]) && i > 0)
                sb.Append(' ');
            sb.Append(str[i]);
        }

        return sb.ToString();
    }





    //==============
    //    Gizmos
    //==============

    public static void DrawRadius(this MonoBehaviour monoBehaviour, Color color, float radius)
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(monoBehaviour.transform.position, radius);
    }

    public static void DrawVelOrDir(this MonoBehaviour monoBehaviour, Color color, Vector3 vector)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(monoBehaviour.transform.position, monoBehaviour.transform.position + vector);
        Gizmos.DrawSphere(monoBehaviour.transform.position + vector, vector.magnitude / 15f);
    }

    public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
    {
#if UNITY_EDITOR
        if (_color != default(Color))
            Handles.color = _color;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (_height - (_radius * 2)) / 2;

            //draw sideways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
            Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
            //draw frontways
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
            Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
            //draw center
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);
        }
#endif
    }

    //==============
    //  Animator
    //==============

    public static AnimationClip GetClip(this Animator animator, string animationName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip;
            }
        }

        Debug.LogError("Could Not Find Animation (" + animationName + ")");
        return null;
    }
    public static bool ContainsClip(this Animator animator, string animationName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animationName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Transitions smoothly to an animation if not already selected or in a transition.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="name"></param>
    /// /// <param name="transitionDurriration"></param>
    public static void SetAnimation(this Animator animator, string name, float transitionDurriration = 0.1f)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(name) && !animator.IsInTransition(0)) animator.CrossFade(name, transitionDurriration, 0);
    }
    public static void ClearAnimationLayer(this Animator animator, int layer, string idleAnimationName, float transitionDurriration = 0.1f)
    {
        if (layer >= animator.layerCount)
        {
            Debug.LogError($"Trying to play animation: {idleAnimationName} on a layer that does not exist on animator: {animator.name}");
            return;
        }
        if (!animator.GetCurrentAnimatorStateInfo(layer).IsName(idleAnimationName) && !animator.IsInTransition(layer)) animator.CrossFade(idleAnimationName, transitionDurriration, layer);
        animator.SetLayerWeight(layer, 0f);
    }
    public static void SetAnimationOnLayer(this Animator animator, int layer, string name, float transitionDurriration = 0.1f)
    {
        if (layer >= animator.layerCount)
        {
            Debug.LogError($"Trying to play animation: {name} on a layer that does not exist on animator: {animator.name}");
            return;
        }
        animator.SetLayerWeight(layer, 1f);
        if (!animator.GetCurrentAnimatorStateInfo(layer).IsName(name) && !animator.IsInTransition(layer)) animator.CrossFade(name, transitionDurriration, layer);
    }

    //==============
    //   Strip
    //==============

    /// <summary>
    /// Destroys all objects on the transform in order of requirements in order to not cause errors. Does not remove the animator.
    /// </summary>
    /// <param name="transform"></param>
    public static void Strip(this Transform transform, Component[] doNotStrip = null)
    {
        //Get Type List
        Component[] componentArray = transform.GetComponents<Component>();
        List<Type> typeList = new List<Type>();
        for (int i = 0; i < componentArray.Length; i++)
        {
            typeList.Add(componentArray[i].GetType());
        }
        //Remove Components in order
        int currentIndex = 0;
        foreach (Type type in FindAllRequirements(typeList))
        {
            bool canStrip = type != typeof(Transform);
            if (doNotStrip != null)
            {
                for (int i = 0; i < doNotStrip.Length; i++)
                {
                    if (type == doNotStrip[i].GetType() && doNotStrip[i].transform == transform) canStrip = false;
                    if (doNotStrip[i].GetType() == typeof(Animator))
                    {
                        ((Animator)doNotStrip[i]).fireEvents = false;
                    }
                }
            }
            if (canStrip)
            {
                for (int i = 0; i < componentArray.Length; i++)
                {
                    if (componentArray[i].GetType() == type)
                    {
                        Component.Destroy(componentArray[i]);
                    }
                }
            }
            currentIndex++;
        }
    }
    public static List<Type> FindAllRequirements(List<Type> types)
    {
        List<Type> requirements = new List<Type>();
        foreach (Type type in types)
        {
            if (!requirements.Contains(type))
            {
                //Go Before Any Of Its Requirements
                int indexToInsert = requirements.Count;
                foreach (RequireComponent requirement in Attribute.GetCustomAttributes(type, typeof(RequireComponent)))
                {
                    bool requiredByRequirements = false;
                    foreach (RequireComponent intiriorRequirement in Attribute.GetCustomAttributes(type, typeof(RequireComponent)))
                    {
                        if (intiriorRequirement.m_Type0 == requirement.m_Type0)
                        {
                            requiredByRequirements = true;
                            break;
                        }
                    }

                    if (requirements.Contains(type) && !requiredByRequirements && requirements.IndexOf(type) < indexToInsert)
                    {
                        indexToInsert = requirements.IndexOf(type);
                    }
                }
                requirements.Insert(indexToInsert, type);
                requirements = FindRequirements(requirements, type);
            }
        }
        requirements.Reverse();
        /*
        if (requirements.Contains(typeof(UniversalAdditionalLightData)))
        {
            requirements.Remove(typeof(UniversalAdditionalLightData));
            requirements.Insert(0, typeof(UniversalAdditionalLightData));
        }
        */
        return requirements;
    }
    public static List<Type> FindRequirements(List<Type> requirements, Type componentType)
    {
        foreach (RequireComponent requirement in Attribute.GetCustomAttributes(componentType, typeof(RequireComponent)))
        {
            if (!requirements.Contains(requirement.m_Type0))
            {
                requirements.Insert(requirements.IndexOf(componentType), requirement.m_Type0);
                requirements = FindRequirements(requirements, requirement.m_Type0);
            }
        }
        return requirements;
    }
#if false //FMOD CODE

    public static float GetVolumeBetter(this FMOD.Studio.Bus bus)
    {
        float volume = 0f;
        bus.getVolume(out volume);
        return volume;
    }
#endif
}
public static class Debuger
{
    public static void ExestenialCrisis(string message)
    {
        Debug.LogError(message);
    }
}