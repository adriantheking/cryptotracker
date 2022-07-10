using CryptoCommon.Repositories.Interfaces;
using MongoDB.Bson;

namespace CryptoCommon.Repositories
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}
