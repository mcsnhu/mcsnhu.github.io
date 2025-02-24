// db.js
const { MongoClient } = require('mongodb');

const uri = 'mongodb://localhost:27017'; // adjust if necessary
const client = new MongoClient(uri);

async function connect() {
  try {
    await client.connect();
    console.log('Connected to MongoDB');
    // Use (or create) a database called "CourseAssistantDB"
    return client.db('CourseAssistantDB');
  } catch (err) {
    console.error('MongoDB connection error:', err);
  }
}

module.exports = { client, connect };
