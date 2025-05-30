const mongoose = require('mongoose');

const userSchema = new mongoose.Schema({
  sqlUserId: { type: Number, unique: true }, 
  userName: String,
  email: String,
  phoneNumber: String,
  dateCreated: Date,
  roles: [String],
  walletBalance: {
    type: Number,
    default: 0
  }
  
});

module.exports = mongoose.model('User', userSchema);
