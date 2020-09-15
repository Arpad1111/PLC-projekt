using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_with_entity
{
    public class DynamicModelChacheKeyFactory:IModelCacheKeyFactory
    {
        public object Create(DbContext context)
          => context is tc_dataCollectionContext dynamicContext
              ? (context.GetType(), dynamicContext.Tablename)
              : (object)context.GetType();
    }
}
