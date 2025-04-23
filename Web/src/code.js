'use strict'

var posicionX=0
var posicionY=0

const controller={
    canvas:null,
    ctx:null,
    imgData:null,
    canvasPositionx:null,
    canvasPositiony:null,    
    canvasMovex:null,
    canvasMovey:null,
    
    platforms : [],
    tresures : [],
    enemies  : [],
    rocks  : [],
    ActualID : -1,
    imgFondo:null,
    imgFondoEx:null,
    imgsMove:null,
    camera: null,

    onload:()=>{
        let canvas = document.getElementById("myCanvas");
        canvas.height=580
        canvas.width=1200
        canvas.style.width=3600
        canvas.style.height=2400
        controller.canvasMovex = 0;
        controller.canvasMovey = 0;
        controller.canvas=canvas;
        controller.ctx = controller.canvas.getContext("2d");
        
        SetupKeyboardEvents();
        SetupMouseEvents(controller.canvas);
    
        Start(controller.canvas);
        Loop();
    }
}

const timer={
    targetdelta : 1 / 60,
    currentdelta : 0,
    time : 0,
    FPS  : 0,
    frames    : 0,
    acumDelta : 0,
    sinceBegining : 0,
    pause : false,
    start : true,
    reStart : false,
    loop:()=>{
        //delta
        const now = Date.now();
        timer.currentdelta = (now - timer.time) / 1000;
        timer.time = now;

        // frames counter
        timer.frames++;
        timer.acumDelta += timer.currentdelta;

        if (timer.acumDelta > 1)
        {
            timer.FPS = timer.frames;
            timer.frames = 0;
            timer.acumDelta -= 1;
        }
        
        if (timer.currentdelta > 100)
            timer.currentdelta = 100;

        if(!timer.paused)
            timer.sinceBegining += timer.delta;
    },
    is_paused:()=>{
        if (Input.IsKeyDown(KEY_PAUSE) || Input.IsKeyDown(KEY_ESCAPE))
        {
            timer.pause = !timer.pause;
        }
    },
    is_start:()=>{
        if (Input.IsKeyDown(KEY_SPACE) && timer.start == true )
        {
            timer.start = !timer.start;
        }
    },
    is_reStart:()=>{
        if (player.isDead && Input.IsKeyDown(KEY_SPACE))
        {
            window.location.reload();
        }
    },
    is_gameEnd:()=>{
        if (player.endGame && Input.IsKeyDown(KEY_SPACE))
        {
            window.location.reload();
        }
    },
    draw:(ctx)=>{
        ctx.fillStyle = "white";
        ctx.font = "12px Arial";
        ctx.fillText("FPS=" + timer.FPS, 10, 30);
        ctx.fillText("delta=" + timer.currentdelta, 10, 50);
        ctx.fillText("currentFPS=" + (1/timer.currentdelta).toFixed(2), 10, 70);
    }
}

//window.requestAnimationFrame =
//        window.requestAnimationFrame ||
//    	window.mozRequestAnimationFrame    ||
//    	window.webkitRequestAnimationFrame ||
//    	window.msRequestAnimationFrame     ||
//    	function (callback) {
//        	window.setTimeout(callback, targetdelta * 1000);
//        }

window.onload = controller.onload;

function Start(canvas)
{
    //player
    player.Start();
    timer.time= Date.now();
    Map.SetMap();
    controller.canvasPositionx = 0;
    controller.canvasPositiony = -1100;
    controller.ctx.translate(controller.canvasPositionx, controller.canvasPositiony);
    //scene objects

    OldMan.Start();

    let imgTres = new Image();
    imgTres.src = "img/Cofre.png";
    controller.tresures.push(new Tresure(560, 1930, imgTres.width, imgTres.height, imgTres),new Tresure(80, 250, imgTres.width, imgTres.height, imgTres));

    let imgRock = new Image();
    imgRock.src = "img/rock.png";
    controller.rocks.push(new Rock(80, 480, 160, 80, imgRock, 0),new Rock(160, 1440, 160, 80, imgRock, 1),
    new Rock(2240, 2080, 160, 80, imgRock, 2),new Rock(3280, 2240, 160, 80, imgRock, 3));

    let imgEnemy = new Image(80,80);
    imgEnemy.src = "img/Slime_azul.png";
    controller.enemies.push(
    new Enemy(1281- imgEnemy.width/2, 2240, imgEnemy.width, imgEnemy.height, imgEnemy, 880),
    new Enemy(832- imgEnemy.width/2, 560, imgEnemy.width, imgEnemy.height, imgEnemy, 800),
    new Enemy(3200 - imgEnemy.width/2, 1360, imgEnemy.width, imgEnemy.height, imgEnemy, 240),);

    controller.enemies.forEach(Enemy => Enemy.Start());

    //background
    controller.imgFondo = new Image(canvas.width,canvas.height);
    controller.imgFondo.src = "img/fondo.png";
    controller.imgFondoEx = new Image(canvas.width,canvas.height);
    controller.imgFondoEx.src = "img/fondo-export.png";
    controller.imgsMove = 0;

        
    //controller.camera = new Camera(player);
}

function Loop ()
{
    timer.loop();
    timer.is_gameEnd();
    timer.is_start();
    timer.is_reStart();
    timer.is_paused();
    
    // Game logic -------------------
    if (!timer.pause && !timer.start)
        Update(timer.currentdelta);

    // Draw the game ----------------
    Draw(controller.ctx);

    Input.PostUpdate();

    // prepare the next loop
    //requestAnimationFrame(Loop);
    setTimeout(Loop,18);
}

function Update(delta)
{  
    // player
    player.Update(controller.canvas,delta,controller.platforms);
    //controller camera
    controller.canvasPositionx += controller.canvasMovex;
    controller.canvasPositiony += controller.canvasMovey;

    
    // parallax
    controller.imgsMove += player.speed/4;
    //scene objects
    controller.tresures.forEach(Tresure => Tresure.Update(delta));
    controller.enemies.forEach(Enemy => Enemy.Update(delta));
    controller.rocks.forEach(Rock => Rock.Update(delta));
    OldMan.Update(delta);
}

function Draw(ctx)
{
    // backgroundd
    //ctx.fillStyle = '#F2E26D';
    //ctx.fillRect(0, 0, canvas.width, canvas.height);
    let {width,height}=controller.canvas;
    ctx.clearRect(0,0,width,height);
    ctx.drawImage(controller.imgFondoEx, 0 + controller.imgsMove, 0);
    ctx.drawImage(controller.imgFondoEx, 1320 + controller.imgsMove, 0);
    ctx.drawImage(controller.imgFondoEx, 2640 + controller.imgsMove, 0);
    ctx.drawImage(controller.imgFondo, 0, 0);
    //if(!controller.imgData){
    //    controller.imgData = ctx.getImageData(0, 0,1,1);
    //    console.log(controller.imgData.data[4]);
    //}

    //console.log(controller.imgData[3]);
    

    // scene objects
    controller.platforms.forEach(Platform => Platform.Draw(ctx));
    controller.tresures.forEach(Tresure => Tresure.Draw(ctx));
    controller.enemies.forEach(Enemy => Enemy.Draw(ctx));
    OldMan.Draw(ctx);

    //player
    player.Draw(ctx);
    Text.Draw(ctx);

    //Start menu
    if (timer.start)
    {
        ctx.fillStyle = "black";
        ctx.globalAlpha = 0.2;
        ctx.fillRect(-controller.canvasPositionx,-controller.canvasPositiony,1200,800);
        ctx.globalAlpha = 1.0;
        ctx.fillStyle = "white";
        ctx.font = "120px Arial ";
        ctx.textAlign = 'center';
        ctx.fillText('Press space to start', -controller.canvasPositionx + controller.canvas.width/2, -controller.canvasPositiony + controller.canvas.width/2 - 240,);
        ctx.textAlign = 'left';
    }
    //dead menu
    else if(player.isDead){
        ctx.fillStyle = "black";
        ctx.globalAlpha = 0.2;
        ctx.fillRect(-controller.canvasPositionx,-controller.canvasPositiony,1200,800);
        ctx.globalAlpha = 1.0;
        ctx.fillStyle = "white";
        ctx.font = "120px Arial ";
        ctx.textAlign = 'center';
        ctx.fillText('You died', -controller.canvasPositionx + controller.canvas.width/2, -controller.canvasPositiony + controller.canvas.width/2 - 260,);
        ctx.font = "80px Arial ";
        ctx.fillText('Press space to restart', -controller.canvasPositionx + controller.canvas.width/2, -controller.canvasPositiony + controller.canvas.width/2 - 160,);
        ctx.textAlign = 'left';
    }
    //End game
    else if(player.endGame){
        ctx.fillStyle = "black";
        ctx.globalAlpha = 0.2;
        ctx.fillRect(-controller.canvasPositionx,-controller.canvasPositiony,1200,800);
        ctx.globalAlpha = 1.0;
        ctx.fillStyle = "white";
        ctx.font = "120px Arial ";
        ctx.textAlign = 'center';
        ctx.fillText('Game Ended', -controller.canvasPositionx + controller.canvas.width/2, -controller.canvasPositiony + controller.canvas.width/2 - 260,);
        ctx.font = "80px Arial ";
        ctx.fillText('Press space to restart', -controller.canvasPositionx + controller.canvas.width/2, -controller.canvasPositiony + controller.canvas.width/2 - 160,);
        ctx.textAlign = 'left';
    }
    //pause menu
    else if (timer.pause)
    {
        ctx.fillStyle = "black";
        ctx.globalAlpha = 0.2;
        ctx.fillRect(-controller.canvasPositionx,-controller.canvasPositiony,1200,800);
        ctx.globalAlpha = 1.0;
        ctx.fillStyle = "white";
        ctx.font = "120px Arial ";
        ctx.textAlign = 'center';
        ctx.fillText('PAUSE', -controller.canvasPositionx + controller.canvas.width/2, -controller.canvasPositiony + controller.canvas.width/2 - 240,);
        ctx.textAlign = 'left';
    }
    else{
		if (posicionX + controller.canvasMovex > 0) controller.canvasMovex =- posicionX
        posicionX += controller.canvasMovex
        
		//console.log(posicionX, "canvasPositionx", controller.canvasPositionx, "canvasMovex",controller.canvasMovex)
        ctx.translate(controller.canvasMovex, controller.canvasMovey);}

    // draw the frame counter
    timer.draw(ctx)    
}