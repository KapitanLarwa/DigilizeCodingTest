using System.Collections.Generic;
using System.Linq;
using Digilize.Domain.Entities;

namespace Digilize.Application.Services
{
    public class ValidationService
    {
        public List<T> ValidateEntities<T>(List<T> entities) where T : EntityBase
        {
            return entities.Where(e => e.Id != Guid.Empty &&
                                       typeof(T).GetProperties().All(p => p.GetValue(e) != null))
                .ToList();
        }
    }
}
