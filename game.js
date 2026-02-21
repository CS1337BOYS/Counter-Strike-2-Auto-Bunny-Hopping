const canvas = document.getElementById('bhopCanvas');
const ctx = canvas.getContext('2d');

// Game State Physics Config
const gravity = 0.6;
const maxVelocityX = 350; 
const defaultVelocityX = 200;
const jumpForce = -10;
const groundY = 250;

let player = {
    x: 50,
    y: groundY - 30,
    width: 30,
    height: 30,
    vx: 0,
    vy: 0,
    isGrounded: true,
    isCrouching: false
};

// Aesthetics & Metrics
let score = 0;
let highestSpeed = 0;
let particles = [];
let platforms = [];
let gameLoopId;
let gameActive = true;
let jumpBufferTime = 0;

// Input State
const keys = {
    space: false,
    ctrl: false
};

// Platform Generator
function generatePlatform(startX) {
    let width = Math.random() * 150 + 100;
    let gap = Math.random() * 100 + 80;
    if (player.vx > 250) gap += 100; // Wider gaps when fast
    
    platforms.push({ x: startX, width: width, height: 50, y: groundY });
    return startX + width + gap;
}

// Initial Level
let nextPlatformX = 0;
for(let i=0; i<10; i++) {
    nextPlatformX = generatePlatform(nextPlatformX);
}

// Input Handlers
window.addEventListener('keydown', (e) => {
    if(e.code === 'Space') {
        keys.space = true;
        jumpBufferTime = 5; // 5 frames of forgiveness for bhop
        e.preventDefault();
    }
    if(e.code === 'ControlLeft' || e.code === 'ControlRight') {
        keys.ctrl = true;
        if(!player.isCrouching) {
            player.isCrouching = true;
            player.height = 15; // Hitbox reduction
            player.y += 15;
            
            // "Long-Jump" velocity boost if triggered right before/during jump
            if(!player.isGrounded && player.vy < 0) {
                 player.vx += 25; 
            }
        }
    }
});

window.addEventListener('keyup', (e) => {
    if(e.code === 'Space') keys.space = false;
    if(e.code === 'ControlLeft' || e.code === 'ControlRight') {
        keys.ctrl = false;
        if(player.isCrouching) {
            player.isCrouching = false;
            player.height = 30; // Restore Hitbox
            player.y -= 15;
        }
    }
});

// Particles
function spawnParticles(x, y, count, color) {
    for(let i=0; i<count; i++) {
        particles.push({
            x: x, y: y,
            vx: (Math.random() - 0.5) * 5,
            vy: (Math.random() - 1) * 5,
            life: 1,
            color: color
        });
    }
}

// Main Loop
function update() {
    if(!gameActive) return;

    // Movement Physics
    player.vy += gravity;
    player.y += player.vy;

    // Auto-scroll screen based on player speed
    let scrollSpeed = player.vx * 0.016; 
    
    // Decrease velocity slowly if grounded (friction loss)
    if(player.isGrounded && player.vx > defaultVelocityX) {
        player.vx *= 0.95; 
    }

    // Platform Collision logic
    player.isGrounded = false;
    let stoodOnPlatform = false;

    for(let i = 0; i < platforms.length; i++) {
        let p = platforms[i];
        p.x -= scrollSpeed; // Scroll world left

        // AABB collision checking
        if (player.x < p.x + p.width &&
            player.x + player.width > p.x &&
            player.y + player.height >= p.y &&
            player.y + player.height <= p.y + player.vy + 2) {
            
            player.y = p.y - player.height;
            player.vy = 0;
            player.isGrounded = true;
            stoodOnPlatform = true;

            // Perfect Bhop check (jump buffer)
            if (jumpBufferTime > 0) {
                // Perfect Bhop! Grant velocity.
                player.vy = jumpForce;
                player.vx = Math.min(player.vx + 15, maxVelocityX);
                player.isGrounded = false;
                spawnParticles(player.x + player.width/2, player.y + player.height, 10, '#FF5D00');
            } else if (!keys.space) {
                // Touched ground without jumping, reset velocity
                player.vx = defaultVelocityX;
            }
        }
    }

    // Jump buffer countdown
    if(jumpBufferTime > 0) jumpBufferTime--;

    // Initial Jump from ground
    if (player.isGrounded && keys.space) {
        player.vy = jumpForce;
        player.isGrounded = false;
    }

    // Death / Fall off map
    if (player.y > canvas.height + 50) {
        gameOver();
    }

    // Keep generating world
    if (platforms[platforms.length-1].x < canvas.width + 200) {
        nextPlatformX -= scrollSpeed; // adjust offset
        nextPlatformX = generatePlatform(platforms[platforms.length-1].x + platforms[platforms.length-1].width + (Math.random() * 100 + 80 + (player.vx > 250 ? 50 : 0)));
    }

    // Clean up old platforms
    if(platforms[0].x + platforms[0].width < -100) {
        platforms.shift();
    }

    // Metrics
    if(player.vx > highestSpeed) highestSpeed = Math.floor(player.vx);
    score += scrollSpeed * 0.1;

    draw();
    gameLoopId = requestAnimationFrame(update);
}

function draw() {
    // Clear
    ctx.fillStyle = '#111';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Draw Platforms
    ctx.fillStyle = '#333';
    ctx.strokeStyle = '#FF5D00';
    ctx.lineWidth = 2;
    for(let p of platforms) {
        ctx.fillRect(p.x, p.y, p.width, p.height);
        ctx.strokeRect(p.x, p.y, p.width, p.height);
    }

    // Draw Player
    ctx.fillStyle = player.isCrouching ? '#FF5D00' : '#FFF';
    ctx.shadowBlur = player.vx > 250 ? 15 : 0;
    ctx.shadowColor = '#FF5D00';
    ctx.fillRect(player.x, player.y, player.width, player.height);
    ctx.shadowBlur = 0;

    // Draw Particles
    for(let i = particles.length-1; i>=0; i--) {
        let p = particles[i];
        p.x += p.vx;
        p.y += p.vy;
        p.life -= 0.05;
        
        ctx.fillStyle = `rgba(255, 93, 0, ${p.life})`;
        ctx.fillRect(p.x, p.y, 4, 4);

        if(p.life <= 0) particles.splice(i, 1);
    }

    // UI
    ctx.fillStyle = 'white';
    ctx.font = '14px Jura, sans-serif';
    ctx.fillText(`VELOCITY: ${Math.floor(player.vx)} u/s`, 10, 25);
    ctx.fillText(`Max Speed: ${highestSpeed}`, 10, 45);
    ctx.fillText(`Score: ${Math.floor(score)}`, canvas.width - 100, 25);

    // Speed Lines Effect
    if(player.vx > 280) {
        ctx.fillStyle = 'rgba(255, 255, 255, 0.1)';
        for(let i=0; i<5; i++) {
            ctx.fillRect(Math.random() * canvas.width, Math.random() * canvas.height, Math.random() * 50 + 20, 1);
        }
    }
}

function gameOver() {
    gameActive = false;
    ctx.fillStyle = 'rgba(0,0,0,0.8)';
    ctx.fillRect(0,0,canvas.width, canvas.height);
    
    ctx.fillStyle = '#FF5D00';
    ctx.font = '30px Jura, sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText('YOU LOST MOMENTUM', canvas.width/2, canvas.height/2 - 20);
    
    ctx.fillStyle = 'white';
    ctx.font = '16px Roboto';
    ctx.fillText(`Score: ${Math.floor(score)} | Max Velocity: ${highestSpeed}`, canvas.width/2, canvas.height/2 + 20);
    ctx.fillText(`Click to Respawn`, canvas.width/2, canvas.height/2 + 60);
    ctx.textAlign = 'left';
}

// Reset Game
canvas.addEventListener('mousedown', () => {
    if(!gameActive) {
        player.y = groundY - 50;
        player.vy = 0;
        player.vx = defaultVelocityX;
        platforms = [];
        nextPlatformX = 0;
        for(let i=0; i<10; i++) nextPlatformX = generatePlatform(nextPlatformX);
        score = 0;
        highestSpeed = 0;
        gameActive = true;
        update();
    }
});

// Start loop
update();
