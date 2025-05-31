require('dotenv').config();
const connectDB = require('./config/db'); 
const ApiResponse = require('./utils/ApiResponse');  
const axios = require('axios');
const express = require('express'); 
const { expressjwt: jwt } = require('express-jwt');
const mongoose = require('mongoose');
const User = require('./models/User');
 
connectDB();
const app = express();
 
app.use(express.json());
 
const checkJwt = jwt({
  secret: process.env.JWT_SECRET,
  audience: process.env.JWT_AUDIENCE,
  issuer: process.env.JWT_ISSUER,
  algorithms: ['HS256'],   
});
 
function checkRole(role) {
  return (req, res, next) => {
    const userRoles = req.auth && req.auth['roles'];
    if (!userRoles || !userRoles.includes(role)) {
      return res.status(403).json({ message: 'Forbidden: Insufficient role' });
    }
    next();
  };
}
 
async function placeBet(userId, gameId, amount) {
  return {
    betId: `bet_${Math.random().toString(36).substring(7)}`,
    userId,
    gameId,
    amount,
    status: 'placed',
    placedAt: new Date().toISOString(),
  };
}

app.get('/', (req, res) => {
  res.send('Casino backend is running');
});

 app.get('/debug/users', async (req, res) => {
  const users = await User.find({});
  res.json(users);
});
 
app.get('/wallet/balance', checkJwt, async (req, res) => {
  try {
    const sqlUserId = Number(req.auth.userId);
    const user = await User.findOne({ sqlUserId }).select('walletBalance');
    console.log('User ID from JWT:', req.auth.userId, typeof req.auth.userId);

    if (!user) {
      return res.status(404).json(new ApiResponse({ success: false, message: 'User not found' }));
    }
    res.json(new ApiResponse({
      success: true,
      message: 'Wallet balance fetched',
      data: { userId: sqlUserId, balance: user.walletBalance }
    }));
  } catch (error) {
    res.status(500).json(new ApiResponse({ success: false, message: 'Internal server error', error: error.message }));
  }
});
 
app.post('/wallet/deposit', checkJwt, async (req, res) => {
  const { amount } = req.body;
  const sqlUserId = Number(req.auth.userId); 

  if (!amount || amount <= 0) {
    return res.status(400).json(new ApiResponse({ success: false, message: 'Amount must be greater than 0' }));
  }

  try {
    const user = await User.findOneAndUpdate(
      { sqlUserId },
      { $inc: { walletBalance: amount } },
      { new: true }
    );

    if (!user) {
      return res.status(404).json(new ApiResponse({ success: false, message: 'User not found zz' }));
    }

    res.json(new ApiResponse({
      success: true,
      message: 'Deposit successful',
      data: { balance: user.walletBalance }
    }));
  } catch (error) {
    res.status(500).json(new ApiResponse({ success: false, message: 'Server error', error: error.message }));
  }
});
 
app.post('/wallet/withdraw', checkJwt, async (req, res) => {
  const { amount } = req.body;
  const sqlUserId = Number(req.auth.userId);

  if (!amount || amount <= 0) {
    return res.status(400).json(new ApiResponse({ success: false, message: 'Amount must be greater than 0' }));
  }

  try {
    const user = await User.findOne({ sqlUserId });

    if (!user) {
      return res.status(404).json(new ApiResponse({ success: false, message: 'User not found grr' }));
    }

    if (user.walletBalance < amount) {
      return res.status(400).json(new ApiResponse({ success: false, message: 'Insufficient balance' }));
    }

    user.walletBalance -= amount;
    await user.save();

    res.json(new ApiResponse({
      success: true,
      message: 'Withdrawal successful',
      data: { balance: user.walletBalance }
    }));
  } catch (error) {
    res.status(500).json(new ApiResponse({ success: false, message: 'Server error', error: error.message }));
  }
});

app.post('/games/slots/play', checkJwt, async (req, res) => {
  const { betAmount } = req.body;
  const sqlUserId = Number(req.auth.userId);

  if (!betAmount || betAmount <= 0) {
    return res.status(400).json(new ApiResponse({ success: false, message: 'Bet amount must be greater than 0' }));
  }

  try {
    const user = await User.findOne({ sqlUserId });
    if (!user) {
      return res.status(404).json(new ApiResponse({ success: false, message: 'User not found' }));
    }

    if (user.walletBalance < betAmount) {
      return res.status(400).json(new ApiResponse({ success: false, message: 'Insufficient balance' }));
    }

    user.walletBalance -= betAmount;

    // 30% chance to win, 2x payout
    const didWin = Math.random() < 0.3;
    const winAmount = didWin ? betAmount * 2 : 0;

    user.walletBalance += winAmount;
    await user.save();

    console.log(`User ${sqlUserId} played slots: bet ${betAmount}, won ${winAmount}`);

    res.json(new ApiResponse({
      success: true,
      message: didWin ? 'YOU WON!' : 'YOU LOST.',
      data: { betAmount, winAmount, finalBalance: user.walletBalance }
    }));
  } catch (error) {
    console.error('Slots play error:', error);
    res.status(500).json(new ApiResponse({ success: false, message: 'Server error', error: error.message }));
  }
});
 
app.post('/login', async (req, res) => {
  try {
    const { email, password } = req.body; 
    if (!email || !password) {
      return res
        .status(400)
        .json(new ApiResponse({ success: false, message: 'Missing credentials' }));
    }
  
    const response = await axios.post('http://identity-service:8081/api/users/signin', { email, password });
    const { token, user } = response.data.data; 
    const sqlUserId = Number(user.id);   

    let mongoUser = await User.findOne({ sqlUserId }); 
    if (!mongoUser) {
      mongoUser = new User({
        sqlUserId,
        userName: user.userName || user.email,  
        email: user.email,
        phoneNumber: user.phoneNumber || '',
        walletBalance: 0,
        roles: user.roles || ['USER'],
        dateCreated: new Date()
      });

      await mongoUser.save();
      console.log(`New MongoDB user created for SQL user ID ${sqlUserId}`);
    }
 
    res.json(
      new ApiResponse({
        success: true,
        message: 'Login successful',
        data: { token, user: mongoUser }
      })
    );
  } catch (err) {
    res
      .status(401)
      .json(
        new ApiResponse({
          success: false,
          message: 'Authentication failed',
          error: err.message
        })
      );
  }
});
 
app.post('/games/roulette/play', checkJwt, async (req, res) => {
  const { betAmount, betChoice } = req.body; // betChoice: number (0-36) or 'red'/'black'
  const sqlUserId = Number(req.auth.userId);

  if (!betAmount || betAmount <= 0) {
    return res.status(400).json(new ApiResponse({ success: false, message: 'Bet amount must be > 0' }));
  }
  if (
    !(
      (typeof betChoice === 'number' && betChoice >= 0 && betChoice <= 36) ||
      (typeof betChoice === 'string' && ['red', 'black'].includes(betChoice.toLowerCase()))
    )
  ) {
    return res.status(400).json(new ApiResponse({ success: false, message: 'Invalid bet choice: must be 0-36 or "red"/"black"' }));
  }

  try {
    const user = await User.findOne({ sqlUserId });
    if (!user) {
      return res.status(404).json(new ApiResponse({ success: false, message: 'User not found' }));
    }
    if (user.walletBalance < betAmount) {
      return res.status(400).json(new ApiResponse({ success: false, message: 'Insufficient balance' }));
    }

    user.walletBalance -= betAmount;

    const spinNumber = Math.floor(Math.random() * 37);
    const redNumbers = [
      1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36
    ];
    let spinColor = 'green';
    if (spinNumber !== 0) {
      spinColor = redNumbers.includes(spinNumber) ? 'red' : 'black';
    }

    let didWin = false;
    let winAmount = 0;

    if (typeof betChoice === 'number') {
      // Bet on digit
      didWin = (betChoice === spinNumber);
      if (didWin) winAmount = betAmount * 35;
    } else {
      // Bet on color (case insensitive)
      didWin = (betChoice.toLowerCase() === spinColor);
      if (didWin) winAmount = betAmount * 2;
    }

    if (didWin) {
      user.walletBalance += winAmount;
    }

    await user.save();

    res.json(new ApiResponse({
      success: true,
      message: didWin
        ? `You won! The ball landed on ${spinNumber} (${spinColor}).`
        : `You lost. The ball landed on ${spinNumber} (${spinColor}).`,
      data: {
        betAmount,
        betChoice,
        spinNumber,
        spinColor,
        winAmount,
        finalBalance: user.walletBalance
      }
    }));
  } catch (error) {
    res.status(500).json(new ApiResponse({ success: false, message: 'Server error', error: error.message }));
  }
});
 
app.post('/games/cointoss/play', checkJwt, async (req, res) => {
  const { betAmount, betChoice } = req.body; // betChoice: 'heads' or 'tails'
  const sqlUserId = Number(req.auth.userId);

  if (!betAmount || betAmount <= 0 || !['heads', 'tails'].includes(betChoice)) {
    return res.status(400).json(new ApiResponse({ success: false, message: 'Invalid bet amount or choice' }));
  }

  try {
    const user = await User.findOne({ sqlUserId });
    if (!user) {
      return res.status(404).json(new ApiResponse({ success: false, message: 'User not found' }));
    }

    if (user.walletBalance < betAmount) {
      return res.status(400).json(new ApiResponse({ success: false, message: 'Insufficient balance' }));
    }

    user.walletBalance -= betAmount;

    const tossResult = Math.random() < 0.5 ? 'heads' : 'tails';
    let winAmount = 0;
    const didWin = (betChoice === tossResult);

    if (didWin) {
      winAmount = betAmount * 2;
      user.walletBalance += winAmount;
    }

    await user.save();

    res.json(new ApiResponse({
      success: true,
      message: didWin ? `You won! The toss was ${tossResult}.` : `You lost! The toss was ${tossResult}.`,
      data: { betAmount, betChoice, tossResult, winAmount, finalBalance: user.walletBalance }
    }));
  } catch (error) {
    res.status(500).json(new ApiResponse({ success: false, message: 'Server error', error: error.message }));
  }
});


const port = process.env.PORT || 3000;
app.listen(port, () => {
  console.log(`Server running on http://localhost:${port}`);
});
