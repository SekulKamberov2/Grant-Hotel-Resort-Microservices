// src/models/User.ts

import mongoose, { Schema, Document } from 'mongoose';

export interface IUser extends Document {
  sqlUserId: number;
  userName: string;
  email: string;
  phoneNumber: string;
  dateCreated: Date;
  roles: string[];
  walletBalance: number;
}

const userSchema: Schema = new Schema({
  sqlUserId: { type: Number, unique: true, required: true },
  userName: { type: String },
  email: { type: String },
  phoneNumber: { type: String },
  dateCreated: { type: Date, default: Date.now },
  roles: [{ type: String }] 
});

const User = mongoose.model<IUser>('User', userSchema);

export default User;
