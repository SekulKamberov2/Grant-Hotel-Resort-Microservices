import express from 'express';
import { Router } from 'express-serve-static-core';
import { signIn } from '../controllers/authController';

const router: Router = express.Router();

router.post('/signin', signIn);   


export default router;
