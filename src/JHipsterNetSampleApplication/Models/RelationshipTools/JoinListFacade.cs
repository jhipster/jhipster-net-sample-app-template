using JHipsterNetSampleApplication.Models.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JHipsterNetSampleApplication.Models.RelationshipTools {
    public class JoinListFacade<TEntity, TOtherEntity, TJoinEntity>
    : IList<TEntity>
    where TJoinEntity : IJoinedEntity<TEntity>, IJoinedEntity<TOtherEntity>, new() {
        private readonly TOtherEntity _ownerEntity;
        private readonly IList<TJoinEntity> _list;

        public JoinListFacade(TOtherEntity ownerEntity, IList<TJoinEntity> list)
        {
            _ownerEntity = ownerEntity;
            _list = list;
        }

        public IEnumerator<TEntity> GetEnumerator() => _list.Select(e => ((IJoinedEntity<TEntity>)e).Join).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TEntity item)
        {
            var entity = new TJoinEntity();
            ((IJoinedEntity<TEntity>)entity).Join = item;
            ((IJoinedEntity<TOtherEntity>)entity).Join = _ownerEntity;
            _list.Add(entity);
        }

        public void Clear() => _list.Clear();

        public bool Contains(TEntity item) => _list.Any(e => Equals(item, e));

        public void CopyTo(TEntity[] array, int arrayIndex) => this.ToList().CopyTo(array, arrayIndex);

        public bool Remove(TEntity item) => _list.Remove(_list.FirstOrDefault(e => Equals(item, e)));

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;        

        private static bool Equals(TEntity item, TJoinEntity e) => Equals(((IJoinedEntity<TEntity>)e).Join, item);

        public TEntity this[int index] {
            get => ((IJoinedEntity<TEntity>)_list[index]).Join;
            set => ((IJoinedEntity<TEntity>)_list[index]).Join = value;
        }

        public int IndexOf(TEntity item)
        {
            var entity = new TJoinEntity();
            ((IJoinedEntity<TEntity>)entity).Join = item;
            ((IJoinedEntity<TOtherEntity>)entity).Join = _ownerEntity;
            return _list.IndexOf(entity);
        }

        public void Insert(int index, TEntity item)
        {
            var entity = new TJoinEntity();
            ((IJoinedEntity<TEntity>)entity).Join = item;
            ((IJoinedEntity<TOtherEntity>)entity).Join = _ownerEntity;
            _list.Insert(index, entity);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
    }
}
