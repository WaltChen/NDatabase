using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Trigger;

namespace Trigger
{
	public class MyTrigger : InsertTrigger
	{
		public int nbInsertsBefore;

		public int nbInsertsAfter;

		public override void AfterInsert(object @object, OID oid)
		{
		    Console.WriteLine("after insert object with id " + oid + "(" + @object.GetType().Name + ")");
			nbInsertsAfter++;
		}

		public override bool BeforeInsert(object @object)
		{
		    Console.WriteLine("trigger before inserting " + @object);
			nbInsertsBefore++;
			return true;
		}
	}
}