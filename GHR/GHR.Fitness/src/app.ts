import dotenv from 'dotenv';
dotenv.config();  

import express from 'express';
import authRoutes from './routes/authRoutes'; 
import planRoutes from './routes/planRoutes'; 

const app = express();


app.use(express.json());
app.use('/api/fitness/users', authRoutes); 
app.use('/api/fitness/plans', planRoutes); 


export default app;
