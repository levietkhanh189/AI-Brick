using ProtoBuf;
using System;
using System.Threading.Tasks;

namespace Battlehub.Storage.Surrogates.UnityEngine
{
    [ProtoContract]
    [Surrogate(typeof(global::UnityEngine.CapsuleCollider), _PROPERTY_INDEX, _TYPE_INDEX)]
    public class CapsuleColliderSurrogate<TID> : ISurrogate<TID> where TID : IEquatable<TID>
    {   
        const int _PROPERTY_INDEX = 12;
        const int _TYPE_INDEX = 114;

        //_PLACEHOLDER_FOR_EXTENSIONS_DO_NOT_DELETE_OR_CHANGE_THIS_LINE_PLEASE


        [ProtoMember(2)]
        public TID id { get; set; }

        [ProtoMember(3)]
        public TID gameObjectId { get; set; }

        [ProtoMember(4)]
        public global::UnityEngine.Vector3 center { get; set; }

        [ProtoMember(5)]
        public global::System.Single radius { get; set; }

        [ProtoMember(6)]
        public global::System.Single height { get; set; }

        [ProtoMember(7)]
        public global::System.Int32 direction { get; set; }

        [ProtoMember(8)]
        public global::System.Boolean enabled { get; set; }

        [ProtoMember(9)]
        public global::System.Boolean isTrigger { get; set; }

        [ProtoMember(10)]
        public global::System.Single contactOffset { get; set; }

        [ProtoMember(11)]
        public TID sharedMaterial { get; set; }

        //[ProtoMember(12)]
        public TID material { get; set; }

        //_PLACEHOLDER_FOR_NEW_PROPERTIES_DO_NOT_DELETE_OR_CHANGE_THIS_LINE_PLEASE

        public ValueTask Serialize(object obj, ISerializationContext<TID> ctx)
        {
            var idmap = ctx.IDMap;

            var o = (global::UnityEngine.CapsuleCollider)obj;
            id = idmap.GetOrCreateID(o);
            gameObjectId = idmap.GetOrCreateID(o.gameObject);
            center = o.center;
            radius = o.radius;
            height = o.height;
            direction = o.direction;
            enabled = o.enabled;
            isTrigger = o.isTrigger;
            contactOffset = o.contactOffset;
            sharedMaterial = idmap.GetOrCreateID(o.sharedMaterial);
            //_PLACEHOLDER_FOR_SERIALIZE_METHOD_BODY_DO_NOT_DELETE_OR_CHANGE_THIS_LINE_PLEASE

            return default;
        }

        public ValueTask<object> Deserialize(ISerializationContext<TID> ctx)
        {
            var idmap = ctx.IDMap;

            var o = idmap.GetComponent<global::UnityEngine.CapsuleCollider, TID>(id, gameObjectId);
            o.center = center;
            o.radius = radius;
            o.height = height;
            o.direction = direction;
            o.enabled = enabled;
            o.isTrigger = isTrigger;
            o.contactOffset = contactOffset;
            o.sharedMaterial = idmap.GetObject<global::UnityEngine.PhysicMaterial>(sharedMaterial);
            //_PLACEHOLDER_FOR_DESERIALIZE_METHOD_BODY_DO_NOT_DELETE_OR_CHANGE_THIS_LINE_PLEASE

            return new ValueTask<object>(o);
        }
    }
}
