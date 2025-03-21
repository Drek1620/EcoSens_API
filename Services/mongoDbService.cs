﻿using EcoSens_API.Models;
using MongoDB.Driver;

namespace EcoSens_API.Services
{
    public class mongoDbService
    {
        private readonly IMongoCollection<RegistroContenedor> _registrosCollection;

        public mongoDbService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoSettings:ConnectionString"]);
            var database = client.GetDatabase(config["MongoSettings:DatabaseName"]);
            _registrosCollection = database.GetCollection<RegistroContenedor>(config["MongoSettings:CollectionName"]);
        }

        public async Task<List<RegistroContenedor>> GetRegistroContenedors(int idContenedor) =>
            await _registrosCollection.Find(p => p.Id_contenedor == idContenedor).ToListAsync();

        public async Task AgregarRegistros(RegistroContenedor registroContenedor) =>
            await _registrosCollection.InsertOneAsync(registroContenedor);
    }
}
