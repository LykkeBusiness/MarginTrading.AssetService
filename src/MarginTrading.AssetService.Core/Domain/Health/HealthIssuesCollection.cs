// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace MarginTrading.AssetService.Core.Domain.Health
{
    public class HealthIssuesCollection : IReadOnlyCollection<HealthIssue>
    {
        public int Count => _list.Count;

        private readonly List<HealthIssue> _list;

        public HealthIssuesCollection()
        {
            _list = new List<HealthIssue>();
        }

        public void Add(string type, string value)
        {
            _list.Add(HealthIssue.Create(type, value));
        }

        public IEnumerator<HealthIssue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}