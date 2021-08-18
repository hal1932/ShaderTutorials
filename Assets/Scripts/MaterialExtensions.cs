using UnityEngine;

public static class MaterialExtensions
{
    public static void SetKeyword(this Material material, string keyword, bool isEnabled)
    {
        if (isEnabled)
        {
            material.EnableKeyword(keyword);
        }
        else
        {
            material.DisableKeyword(keyword);
        }
    }
}
