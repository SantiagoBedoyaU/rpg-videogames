using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Linq;

public class BuildGoblinEnemy
{
    private const string AssetPackPath = "Assets/Dark fantasy - popular enemies- Free Sample/Goblin";
    private const string AnimationsPath = "Assets/Animations/Goblin";
    private const string PrefabOutputPath = "Assets/Prefabs/Goblin.prefab";
    private const string ControllerPath = "Assets/Animations/Goblin/Goblin.controller";

    [MenuItem("Tools/Integrar Goblin como Enemigo")]
    static void Build()
    {
        try
        {
            CreateXpOrbParticles();
            CreateAnimatorController();

            GameObject prefab = CreateGoblinPrefab();

            if (prefab != null)
            {
                Debug.Log("Prefab del Goblin creado exitosamente en: Assets/Prefabs/Goblin.prefab");
                Debug.Log("Ahora arrastra el prefab Goblin al array enemyPrefabs del EnemySpawner en la escena.");
                EditorGUIUtility.PingObject(prefab);
            }

            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al integrar el Goblin: " + e.Message + "\n" + e.StackTrace);
        }
    }

    static void CreateAnimatorController()
    {
        var idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AssetPackPath}/Animations/Goblin_Idle.anim");
        var walkClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AssetPackPath}/Animations/Goblin_Walking.anim");
        var runClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AssetPackPath}/Animations/Goblin_Running.anim");
        var attackClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AssetPackPath}/Animations/Goblin_Attack.anim");
        var dieClip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{AssetPackPath}/Animations/Goblin_Dying.anim");
         // --- Deshabilitar looping en animaciones que no deben repetirse ---
        var dieSettings = AnimationUtility.GetAnimationClipSettings(dieClip);
        dieSettings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(dieClip, dieSettings);
        var attackSettings = AnimationUtility.GetAnimationClipSettings(attackClip);
        attackSettings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(attackClip, attackSettings);

         // --- Animation Event para causar daño ---
        AnimationEvent damageEvent = new AnimationEvent();
        damageEvent.functionName = "CausarDano";
        damageEvent.time = attackClip.length * 0.5f;
        var existingEvents = AnimationUtility.GetAnimationEvents(attackClip);
        var newEvents = new AnimationEvent[existingEvents.Length + 1];
        existingEvents.CopyTo(newEvents, 0);
        newEvents[existingEvents.Length] = damageEvent;
        AnimationUtility.SetAnimationEvents(attackClip, newEvents);
        EditorUtility.SetDirty(attackClip);
        

        if (idleClip == null || walkClip == null || attackClip == null || dieClip == null)
        {
            Debug.LogError("No se encontraron los animation clips del Goblin en: " + AssetPackPath + "/Animations/");
            return;
        }

        var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        if (controller == null)
        {
            Debug.LogError("No se pudo crear el AnimatorController en: " + ControllerPath);
            return;
        }

        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("attack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);

        var layer = controller.layers[0];
        var stateMachine = layer.stateMachine;

        // Remove default state
        if (stateMachine.states.Length > 0)
        {
            var defaultStates = stateMachine.states.Where(s => s.state.name == "New State" || s.state.name.StartsWith("Any State")).ToArray();
            foreach (var s in defaultStates)
            {
                stateMachine.RemoveState(s.state);
            }
        }

        var idleState = stateMachine.AddState("Goblin_Idle");
        idleState.motion = idleClip;

        var walkState = stateMachine.AddState("Goblin_Walking");
        walkState.motion = walkClip;

        var runState = stateMachine.AddState("Goblin_Running");
        runState.motion = runClip;

        var attackState = stateMachine.AddState("Goblin_Attack");
        attackState.motion = attackClip;

        var dieState = stateMachine.AddState("Goblin_Dying");
        dieState.motion = dieClip;

        stateMachine.defaultState = idleState;

        // Idle -> Walk (isWalking = true)
        var idleToWalk = idleState.AddTransition(walkState);
        idleToWalk.AddCondition(AnimatorConditionMode.If, 0, "isWalking");
        idleToWalk.duration = 0.1f;
        idleToWalk.hasExitTime = false;

        // Idle -> Attack (attack trigger)
        var idleToAttack = idleState.AddTransition(attackState);
        idleToAttack.AddCondition(AnimatorConditionMode.If, 0, "attack");
        idleToAttack.duration = 0.1f;
        idleToAttack.hasExitTime = false;

        // Idle -> Die (Die trigger - any state should transition to Die)
        var idleToDie = idleState.AddTransition(dieState);
        idleToDie.AddCondition(AnimatorConditionMode.If, 0, "Die");
        idleToDie.duration = 0.1f;
        idleToDie.hasExitTime = false;

        // Walk -> Idle (isWalking = false)
        var walkToIdle = walkState.AddTransition(idleState);
        walkToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isWalking");
        walkToIdle.duration = 0.1f;
        walkToIdle.hasExitTime = false;

        // Walk -> Attack (attack trigger)
        var walkToAttack = walkState.AddTransition(attackState);
        walkToAttack.AddCondition(AnimatorConditionMode.If, 0, "attack");
        walkToAttack.duration = 0.1f;
        walkToAttack.hasExitTime = false;

        // Walk -> Die
        var walkToDie = walkState.AddTransition(dieState);
        walkToDie.AddCondition(AnimatorConditionMode.If, 0, "Die");
        walkToDie.duration = 0.1f;
        walkToDie.hasExitTime = false;

        // Attack -> Idle (via exit time)
        var attackToIdle = attackState.AddTransition(idleState);
        attackToIdle.hasExitTime = true;
        attackToIdle.exitTime = 0.75f;
        attackToIdle.duration = 0.1f;

        // Attack -> Die
        var attackToDie = attackState.AddTransition(dieState);
        attackToDie.AddCondition(AnimatorConditionMode.If, 0, "Die");
        attackToDie.duration = 0.1f;
        attackToDie.hasExitTime = false;

        // Also create transitions from Run state (for when the enemy is running)
        var runToIdle = runState.AddTransition(idleState);
        runToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isWalking");
        runToIdle.duration = 0.1f;
        runToIdle.hasExitTime = false;

        var runToWalk = runState.AddTransition(walkState);
        runToWalk.AddCondition(AnimatorConditionMode.If, 0, "isWalking");
        runToWalk.duration = 0.1f;
        runToWalk.hasExitTime = false;

        var runToAttack = runState.AddTransition(attackState);
        runToAttack.AddCondition(AnimatorConditionMode.If, 0, "attack");
        runToAttack.duration = 0.1f;
        runToAttack.hasExitTime = false;

        var runToDie = runState.AddTransition(dieState);
        runToDie.AddCondition(AnimatorConditionMode.If, 0, "Die");
        runToDie.duration = 0.1f;
        runToDie.hasExitTime = false;

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();

        Debug.Log("AnimatorController del Goblin creado en: " + ControllerPath);
    }

    static void CreateXpOrbParticles()
    {
        string prefabPath = "Assets/Prefabs/DeathXpOrbs.prefab";
        var material = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/Sherbbs Particle Collection/Materials/VertexUnlitGlowRoundedSquare.mat");
        if (material == null)
        {
            Debug.LogError("No se encontró material VertexUnlitGlowRoundedSquare");
            return;
        }
        GameObject go = new GameObject("DeathXpOrbs", typeof(ParticleSystem));
        var ps = go.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.duration = 0.5f;
        main.startLifetime = 1.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
        main.startSize = 0.15f;
        main.gravityModifier = -0.3f;
        main.maxParticles = 20;
        main.startColor = new Color(1f, 0.85f, 0.1f);
        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 15, 20)
        });
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.y = 0.5f;
        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.2f;
        noise.frequency = 1.5f;
        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = material;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 1;
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);
        Debug.Log("DeathXpOrbs prefab creado en " + prefabPath);
    }

    static GameObject CreateGoblinPrefab()
    {
        var sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{AssetPackPath}/Goblin_Thief.prefab");
        if (sourcePrefab == null)
        {
            Debug.LogError("No se encontró Goblin_Thief.prefab en: " + AssetPackPath);
            return null;
        }

        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            Debug.LogError("Ejecuta primero la creación del AnimatorController.");
            return null;
        }

        // Instantiate the source prefab
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
        if (instance == null)
        {
            Debug.LogError("No se pudo instanciar el prefab fuente.");
            return null;
        }

        instance.name = "Goblin";

        // Configure Animator
        var animator = instance.GetComponent<Animator>();
        if (animator == null) animator = instance.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        // Add/configure Rigidbody2D
        var rb = instance.GetComponent<Rigidbody2D>();
        if (rb == null) rb = instance.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        // Add CapsuleCollider2D - try to auto-size from renderer bounds
        var capsule = instance.GetComponent<CapsuleCollider2D>();
        if (capsule == null) capsule = instance.AddComponent<CapsuleCollider2D>();

        // Try to calculate bounds from child SpriteRenderers
        Bounds bounds = new Bounds(instance.transform.position, Vector3.zero);
        bool hasBounds = false;
        foreach (var sr in instance.GetComponentsInChildren<SpriteRenderer>())
        {
            if (!hasBounds)
            {
                bounds = sr.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(sr.bounds);
            }
        }

        if (hasBounds)
        {
            Vector3 localSize = instance.transform.InverseTransformVector(bounds.size);
            capsule.size = new Vector2(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y));
            Vector3 localCenter = instance.transform.InverseTransformPoint(bounds.center);
            capsule.offset = localCenter;
        }
        else
        {
            capsule.size = new Vector2(0.5f, 0.8f);
            capsule.offset = new Vector2(0, -0.1f);
        }

        capsule.direction = CapsuleDirection2D.Vertical;

        // Add EnemyAI
        var ai = instance.GetComponent<EnemyAI>();
        if (ai == null) ai = instance.AddComponent<EnemyAI>();
        ai.stopDistance = 0.5f;
        ai.attackCooldown = 1.5f;
        ai.attackDamage = 10f;
        ai.health = 200f;
        ai.scoreValue = 10;

        var particlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DeathXpOrbs.prefab");
        if (particlePrefab != null)
        {
            var so = new SerializedObject(ai);
            so.Update();
            var prop = so.FindProperty("deathParticlePrefab");
            if (prop != null)
            {
                prop.objectReferenceValue = particlePrefab;
                so.ApplyModifiedProperties();
            }
        }

        // Add EnemyPathfinding
        var pathfinding = instance.GetComponent<EnemyPathfinding>();
        if (pathfinding == null) pathfinding = instance.AddComponent<EnemyPathfinding>();

        // Save as prefab asset (overwrite if exists)
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(instance, PrefabOutputPath);
        if (savedPrefab == null)
        {
            Debug.LogError("No se pudo guardar el prefab en: " + PrefabOutputPath);
            Object.DestroyImmediate(instance);
            return null;
        }

        // Clean up scene instance
        Object.DestroyImmediate(instance);

        return savedPrefab;
    }
}
