// This file is autogenerated using Battlehub.Storage.ObjectEnumeratorFactoryGen
namespace Battlehub.Storage
{
    public class ObjectEnumeratorFactory : ObjectEnumeratorFactoryBase
    {
        public ObjectEnumeratorFactory(ITypeMap typeMap) : base(typeMap)
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            Register(typeof(global::UnityEngine.BoxCollider), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.BoxColliderEnumerator));
            Register(typeof(global::UnityEngine.Camera), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.CameraEnumerator));
            Register(typeof(global::UnityEngine.Canvas), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.CanvasEnumerator));
            Register(typeof(global::UnityEngine.CapsuleCollider), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.CapsuleColliderEnumerator));
            Register(typeof(global::UnityEngine.Collider), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.ColliderEnumerator));
            Register(typeof(global::UnityEngine.DetailPrototype), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.DetailPrototypeEnumerator));
            Register(typeof(global::UnityEngine.Light), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.LightEnumerator));
            Register(typeof(global::UnityEngine.Material), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.MaterialEnumerator));
            Register(typeof(global::UnityEngine.MeshCollider), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.MeshColliderEnumerator));
            Register(typeof(global::UnityEngine.MeshFilter), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.MeshFilterEnumerator));
            Register(typeof(global::UnityEngine.MeshRenderer), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.MeshRendererEnumerator));
            Register(typeof(global::UnityEngine.SceneManagement.Scene), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.SceneManagement.SceneEnumerator));
            Register(typeof(global::UnityEngine.SkinnedMeshRenderer), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.SkinnedMeshRendererEnumerator));
            Register(typeof(global::UnityEngine.Skybox), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.SkyboxEnumerator));
            Register(typeof(global::UnityEngine.SphereCollider), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.SphereColliderEnumerator));
            Register(typeof(global::UnityEngine.Sprite), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.SpriteEnumerator));
            Register(typeof(global::UnityEngine.Terrain), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.TerrainEnumerator));
            Register(typeof(global::UnityEngine.TerrainCollider), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.TerrainColliderEnumerator));
            Register(typeof(global::UnityEngine.TerrainData), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.TerrainDataEnumerator));
            Register(typeof(global::UnityEngine.TerrainLayer), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.TerrainLayerEnumerator));
            Register(typeof(global::UnityEngine.TreePrototype), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.TreePrototypeEnumerator));
            Register(typeof(global::UnityEngine.Events.UnityEvent), typeof(global::Battlehub.Storage.Enumerators.UnityEngine.Events.UnityEventEnumerator));
        }
    }
}