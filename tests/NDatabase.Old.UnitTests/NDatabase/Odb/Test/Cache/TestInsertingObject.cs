using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Layers.Layer3.Engine;
using NDatabase2.Odb.Core.Oid;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Cache
{
    [TestFixture]
    public class TestInsertingObject : ODBTest
    {
        [Test]
        public virtual void Test2()
        {
            var cache = (IOdbCache) new OdbCache();
            var s1 = "ola1";
            var s2 = "ola2";
            var s3 = "ola3";
            for (var i = 0; i < 1000 * 3; i += 3)
            {
                cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(i + 1), null);
                cache.StartInsertingObjectWithOid(s2, OIDFactory.BuildObjectOID(i + 2), null);
                cache.StartInsertingObjectWithOid(s3, OIDFactory.BuildObjectOID(i + 3), null);
            }
            
            cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(1), null);
            cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(1), null);
            cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(1), null);
            cache.StartInsertingObjectWithOid(s2, OIDFactory.BuildObjectOID(2), null);
            cache.StartInsertingObjectWithOid(s3, OIDFactory.BuildObjectOID(3), null);
            AssertTrue(cache.IdOfInsertingObject(s1) != StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s2) != StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s3) != StorageEngineConstant.NullObjectId);
        }
    }
}