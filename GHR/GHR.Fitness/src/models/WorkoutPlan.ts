import mongoose from 'mongoose';

const ExerciseSchema = new mongoose.Schema({
  name: String,
  sets: Number,
  reps: Number,
  rest: String
});

const DayPlanSchema = new mongoose.Schema({
  day: String, // Monday
  muscleGroup: String, // Chest & Biceps
  exercises: [ExerciseSchema]
});

const WorkoutPlanSchema = new mongoose.Schema({
  userId: { type: Number, required: true },
  weekName: String, // Classic Push-Pull-Legs
  days: [DayPlanSchema]
});

const WorkoutPlan = mongoose.model('WorkoutPlan', WorkoutPlanSchema);

export default WorkoutPlan;
