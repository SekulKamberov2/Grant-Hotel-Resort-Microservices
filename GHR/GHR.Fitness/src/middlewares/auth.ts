import { Request, Response, NextFunction } from 'express';
import jwt from 'jsonwebtoken';

const JWT_SECRET = 'super-secret-key-diojszh$%^$*^^&TYGxduiashauish$@#%#^$&TIGYOTF%$E#^%$%^$*^^&TYGYF^$E#$%E%$ERGFJHYGUILasdhcsjkfhdsjgfd';

interface JwtPayload {
  userId: number;
  email?: string;
  roles?: string[];
}

declare global {
  namespace Express {
    interface Request {
      user?: JwtPayload;
    }
  }
}

export const authenticateToken = (req: Request, res: Response, next: NextFunction) => {
  const authHeader = req.headers['authorization'];

  if (!authHeader || !authHeader.startsWith('Bearer ')) {
    return res.status(401).json({ success: false, message: 'Missing or invalid token' });
  }

  const token = authHeader.split(' ')[1];

  try {
    const payload = jwt.verify(token, JWT_SECRET) as JwtPayload;

    // Normalize userId to number
    req.user = {
      ...payload,
      userId: Number(payload.userId)
    };

    next();
  } catch (err) {
    return res.status(403).json({ success: false, message: 'Invalid or expired token' });
  }
};

