import { Request, Response } from 'express';
import WorkoutPlan from '../models/WorkoutPlan';

export const createPlan = async (req: Request, res: Response) => {
  try {
    const userId = req.user?.userId; 
  
    const planData = {
      userId,
      weekName: req.body.weekName || '',
      days: [
        {
          day: req.body.weekDay,
          muscleGroup: req.body.muscleGroup,
          exercises: req.body.exercises.map((ex: any) => ({
            ...ex,
            rest: ex.rest.toString()  
          }))
        }
      ]
    };

    const plan = await WorkoutPlan.create(planData);
    res.status(201).json(plan);
  } catch (err) {
    res.status(500).json({ success: false, message: 'Failed to create plan', error: err });
  }
};


export const getAllPlans = async (req: Request, res: Response) => {
  try {
    const userId = req.user?.userId;
    const plans = await WorkoutPlan.find({ userId });
    res.json(plans);
  } catch (err) {
    res.status(500).json({ success: false, message: 'Failed to fetch plans', error: err });
  }
}; 

export const getPlansByUserId = async (req: Request, res: Response) => {
  try {
    const userIdFromParams = parseInt(req.params.userId);
    const userIdFromToken = req.user?.userId;
 
    if (userIdFromParams !== Number(userIdFromToken)) {
      return res.status(403).json({ success: false, message: 'Forbidden: User ID mismatch' });
    }

    const plans = await WorkoutPlan.find({ userId: userIdFromParams });
    res.json({ success: true, data: plans });
  } catch (err) {
    res.status(500).json({ success: false, message: 'Failed to fetch user plans', error: err });
  }
};


//userId + planId
export const updatePlanByUser = async (req: Request, res: Response) => {
  try {
    const userIdFromParams = parseInt(req.params.userId);
    const userIdFromToken = req.user?.userId;
    const { planId } = req.params;

    if (userIdFromParams !== Number(userIdFromToken)) {
      return res.status(403).json({ success: false, message: 'Forbidden: User ID mismatch' });
    }

    const updated = await WorkoutPlan.findOneAndUpdate(
      { _id: planId, userId: userIdFromParams },
      req.body,
      { new: true }
    );

    if (!updated) {
      return res.status(404).json({ success: false, message: 'Plan not found or unauthorized' });
    }

    res.json({ success: true, data: updated });
  } catch (err) {
    res.status(500).json({ success: false, message: 'Error updating plan', error: err });
  }
};

export const deletePlanByUser = async (req: Request, res: Response) => {
  try {
    const userIdFromParams = parseInt(req.params.userId);
    const userIdFromToken = req.user?.userId;
    const { planId } = req.params;

    if (userIdFromParams !== Number(userIdFromToken)) {
      return res.status(403).json({ success: false, message: 'Forbidden: User ID mismatch' });
    }

    const deleted = await WorkoutPlan.findOneAndDelete({ _id: planId, userId: userIdFromParams });

    if (!deleted) {
      return res.status(404).json({ success: false, message: 'Plan not found or unauthorized' });
    }

    res.json({ success: true, message: 'Plan deleted' });
  } catch (err) {
    res.status(500).json({ success: false, message: 'Error deleting plan', error: err });
  }
};

