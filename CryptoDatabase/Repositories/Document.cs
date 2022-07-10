using CryproDatabase.Repositories.Interfaces;
using MongoDB.Bson;

namespace CryptoDatabase.Repositories
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}
