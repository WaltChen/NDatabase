using NDatabase.Odb;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;

namespace Test.Odb.Test.Query.Criteria
{
	/// <author>olivier</author>
	[TestFixture]
    public class TestQueryOrderBy : ODBTest
	{
		[Test]
        public virtual void Test1()
		{
			string baseName = "testqueryorderby1.db";
			IOdb odb = Open(baseName);
			odb.Store(new Class1("c1"));
			odb.Store(new Class1("c1"));
			odb.Store(new Class1("c2"));
			odb.Store(new Class1("c2"));
			odb.Store(new Class1("c3"));
			odb.Store(new Class1("c4"));
			odb.Close();
			odb = Open(baseName);
			IQuery q = new CriteriaQuery(typeof(Class1));
			q.OrderByAsc("name");
			IObjects<Class1> objects = odb.GetObjects<Class1>(q);
			AssertEquals(6, objects.Count);
			while (objects.HasNext())
			{
				System.Console.Out.WriteLine(objects.Next());
			}
			// println(objects);
			odb.Close();
		}

		[Test]
        public virtual void Test2()
		{
            string baseName = "testqueryorderby2.db";
			IOdb odb = Open(baseName);
			odb.Store(new Class1("c1"));
			odb.Store(new Class1("c1"));
			odb.Store(new Class1("c2"));
			odb.Store(new Class1("c2"));
			odb.Store(new Class1("c3"));
			odb.Store(new Class1("c4"));
			odb.Close();
			odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof(Class1));
			// q.orderByAsc("name");
			IObjects<Class1> objects = odb.GetObjects<Class1>(q);
			AssertEquals(6, objects.Count);
			Println(objects);
			odb.Close();
		}

		[Test]
        public virtual void Test3()
		{
            string baseName = "testqueryorderby3.db";
			IOdb odb = Open(baseName);
			int size = 500;
			for (int i = 0; i < size; i++)
			{
				odb.Store(new Class1("c1"));
			}
			for (int i = 0; i < size; i++)
			{
				odb.Store(new Class1("c2"));
			}
			odb.Close();
			odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof(Class1));
			// q.orderByAsc("name");
			IObjects<Class1> objects = odb.GetObjects<Class1>(q);
			AssertEquals(size * 2, objects.Count);
			for (int i = 0; i < size; i++)
			{
				Class1 c1 = (Class1
					)objects.Next();
				AssertEquals("c1", c1.GetName());
			}
			for (int i = 0; i < size; i++)
			{
				Class1 c1 = (Class1
					)objects.Next();
				AssertEquals("c2", c1.GetName());
			}
			odb.Close();
		}

		[Test]
        public virtual void Test4()
		{
            string baseName = "testqueryorderby4.db";
			IOdb odb = Open(baseName);
			int size = 5;
			for (int i = 0; i < size; i++)
			{
				odb.Store(new VO.Login.Function("f" + (i + 1)));
			}
			odb.Close();
			odb = Open(baseName);
            IQuery q = new CriteriaQuery(typeof(Class1));
			// q.orderByAsc("name");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 2);
			AssertEquals(2, objects.Count);
			odb.Close();
			odb = Open(baseName);
            q = new CriteriaQuery(typeof(VO.Login.Function));
			q.OrderByAsc("name");
			objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 2);
			AssertEquals(2, objects.Count);
			odb.Close();
			odb = Open(baseName);
            q = new CriteriaQuery(typeof(VO.Login.Function));
			q.OrderByDesc("name");
			objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 2);
			AssertEquals(2, objects.Count);
			odb.Close();
		}

		[Test]
        public virtual void Test51()
		{
            string baseName = "testqueryorderby51.db";
			IOdb odb = Open(baseName);
			odb.Store(new VO.Login.Function("Not Null"));
			odb.Store(new VO.Login.Function(null));
			odb.Close();
			odb = Open(baseName);
			IQuery q = new CriteriaQuery(Where.IsNotNull("name"));
			// q.orderByAsc("name");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			odb.Close();
			AssertEquals(1, objects.Count);
		}

		[Test]
        public virtual void Test5()
		{
            string baseName = "testqueryorderby5.db";
			IOdb odb = Open(baseName);
			int size = 5;
			for (int i = 0; i < size; i++)
			{
				odb.Store(new VO.Login.Function("f1"));
			}
			odb.Store(new VO.Login.Function(null));
			odb.Close();
			odb = Open(baseName);
			IQuery q = new CriteriaQuery(Where.IsNotNull("name"));
			// q.orderByAsc("name");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			AssertEquals(size, objects.Count);
			odb.Close();
			odb = Open(baseName);
			q = new CriteriaQuery(Where.IsNotNull("name"));
			q.OrderByAsc("name");
			objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			AssertEquals(5, objects.Count);
			odb.Close();
			odb = Open(baseName);
			q = new CriteriaQuery(Where.IsNotNull("name"));
			q.OrderByDesc("name");
			objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			AssertEquals(5, objects.Count);
			odb.Close();
		}

		[Test]
        public virtual void Test6()
		{
            string baseName = "testqueryorderby6.db";
			IOdb odb = Open(baseName);
			int size = 5;
			for (int i = 0; i < size; i++)
			{
				odb.Store(new VO.Login.Function("f1"));
			}
			odb.Store(new VO.Login.Function(null));
			odb.Close();
			odb = Open(baseName);
			IQuery q = new CriteriaQuery(Where.IsNull("name"));
			// q.orderByAsc("name");
			IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			AssertEquals(1, objects.Count);
			odb.Close();
			odb = Open(baseName);
			q = new CriteriaQuery(Where.IsNull("name"));
			q.OrderByAsc("name");
			objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			AssertEquals(1, objects.Count);
			odb.Close();
			odb = Open(baseName);
			q = new CriteriaQuery(Where.IsNull("name"));
			q.OrderByDesc("name");
			objects = odb.GetObjects<VO.Login.Function>(q, true, 0, 10);
			AssertEquals(1, objects.Count);
			odb.Close();
		}
	}
}
