import express from 'express';
import { createPlan, getAllPlans, getPlansByUserId, 
    updatePlanByUser, deletePlanByUser} from '../controllers/planController';
import { authenticateToken } from '../middlewares/auth';
const router = express.Router();

router.use(authenticateToken);

router.post('/', createPlan);        
router.get('/', getAllPlans);  

//by userId
router.get('/user/:userId', getPlansByUserId);
router.put('/user/:userId/:planId', updatePlanByUser);
router.delete('/user/:userId/:planId', deletePlanByUser);

export default router;
