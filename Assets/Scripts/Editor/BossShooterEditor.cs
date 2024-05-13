using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossShooter))]
[CanEditMultipleObjects]
public class BossShooterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BossShooter item = (BossShooter)target;
        item.shots_a_sec = EditorGUILayout.FloatField("Shots A sec", item.shots_a_sec);
        item.attack_type = (BossShooter.AttackType)EditorGUILayout.EnumPopup("Type of attack", item.attack_type);
        switch(item.attack_type)
        {
            case BossShooter.AttackType.Straight:
                item.power = EditorGUILayout.FloatField("Power", item.power);
                item.aim_on_player = EditorGUILayout.Toggle("Aim On Player?", item.aim_on_player);
                if (!item.aim_on_player)
                    item.init_aim = EditorGUILayout.Vector2Field("Where to aim", item.init_aim);
                break;
            case BossShooter.AttackType.ShotGun:
                item.shotgun_projectiles = EditorGUILayout.IntField("How many projectiles", item.shotgun_projectiles);
                item.shotgun_spread_angle = EditorGUILayout.IntField("Spread angle", item.shotgun_spread_angle);
                item.power = EditorGUILayout.FloatField("Power", item.power);
                item.aim_on_player = EditorGUILayout.Toggle("Aim On Player?", item.aim_on_player);
                if (!item.aim_on_player)
                    item.init_aim = EditorGUILayout.Vector2Field("Where to aim", item.init_aim);
                break;
            case BossShooter.AttackType.Wave:
                item.wave_len = EditorGUILayout.Slider("Wave length", item.wave_len, 0, 2);
                break;
            case BossShooter.AttackType.Arc:
                item.init_aim = EditorGUILayout.Vector2Field("How high to shoot", item.init_aim);
                break;
        }
        item.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", item.projectile,typeof(GameObject),false);
        item.active = EditorGUILayout.Toggle("Active", item.active);
    }
}
