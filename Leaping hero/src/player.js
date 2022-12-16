
var player = {
    // position and size
    x: 0, 
    y: 0,
    width: 100,
    height: 20,

    // movement
    speed: 0,
    directionFix: 0.6,
    fallspeed: 2,
    maxspeed: 10,

    //animations
    posXanim: 0,
    posYanim: 0,
    timeAnim: 0,

    //basic bools
    canMove: true,
    isInGround: true,
    isKeyLeft: false,
    isDead: false,

    //mechanics
    hasDobleJump: false,
    canDoubleJump: false,
    hasDash: false,
    isInDash: false,
    canDash: false,
    dashCD: 0.08,

    endGame: false,

    //checkpoints
    checkpoints:[
        480,1440,
        2080,2080,
        1280,160,
        3440,880

    ],

    ligth: true,

    imageplayer: new Image(),


    Start: function () {
        imgplayer = new Image(80,80);
        imgplayer.src = "img/Personaje.png";

        this.width = imgplayer.width;
        this.height = imgplayer.height;

        this.x = 480;
        this.y = 1440;

    },

    Update: function (canvas,deltaTime,platforms) {

        this.Actions(deltaTime);

        this.Collision(platforms);
        
        this.x += this.speed;
        this.y += this.fallspeed;

        //top cap
        if(this.y < 0) 
            this.y = 0;
        
        // timer dash
        if(this.isInDash == true){
            if(this.dashCD <= 0){
                this.dashCD  = 0.08;
                this.isInDash = false;
                if(this.isKeyLeft)
                    this.speed += 30;
                else this.speed -= 30;
                    this.canMove = true;
            }else this.dashCD -= deltaTime;
        }

        this.AnimationDirector(deltaTime);

        this.cameraController();

        
    },

    cameraController: function(){

        // x cap
        if(controller.canvasPositionx < 0 && controller.canvasPositionx > -3600 + 1200)
        {
            controller.canvasMovex = -this.speed;
        }
        else if(this.x > 480 && controller.canvasPositionx >= 0 ){
            controller.canvasPositionx = -1;
            controller.canvasMovex = 0;
        }
        else if(this.x <  3600 - 480  && controller.canvasPositionx <= -3600 + 1200 ){
            controller.canvasPositionx = -3599 + 1180;
            controller.canvasMovex = 00;
        }
        else  controller.canvasMovex = 0;

        // y cap
        if(controller.canvasPositiony < 0 && controller.canvasPositiony > -2400 + 560 )
        {
            controller.canvasMovey = -this.fallspeed;
        }
        else if(this.y > 380 && controller.canvasPositiony >= 0 ){
            controller.canvasPositiony = -1;
            controller.canvasMovey = 0;
        }
        else if(this.y <  2400 - 120  && controller.canvasPositiony <= -2400 + 560 ){
            controller.canvasPositiony = -2399 + 560;
            controller.canvasMovey = 0;
        }
        else  controller.canvasMovey = 0;
    },

    hasDied: function(){
        this.isDead = true;
        this.canMove = false;
        this.speed = 0;
    },

    SetIsInGround: function(){
        this.fallspeed = 0;
        this.isInGround = true;
        this.hasDobleJump = true;
        this.hasDash = true;
    },

    Dash: function(){
        this.canMove = false;
        this.hasDash = false;
        if(this.isKeyLeft)
            this.speed -= 30;
        else this.speed += 30;
        this.fallspeed = 0;
        this.isInDash = true;
    },

    //input actions manager
    Actions: function(deltaTime){
        if(this.canMove){
            // move right and left
            if(!Input.IsKeyPressed(KEY_LEFT) && !Input.IsKeyPressed(KEY_RIGHT) || Input.IsKeyPressed(KEY_LEFT) && Input.IsKeyPressed(KEY_RIGHT))
                this.speed *= this.directionFix;
            else if(Input.IsKeyPressed(KEY_LEFT)){
                this.speed --;
                this.isKeyLeft = true;
            }
            else if(Input.IsKeyPressed(KEY_RIGHT)){
                this.speed ++;
                this.isKeyLeft = false;
            }

                

            // jump
            if(Input.IsKeyDown(KEY_UP))
            {
                if(this.isInGround){
                    this.fallspeed = -30;
                    this.isInGround = false;
                }else if(this.canDoubleJump){
                    if(this.hasDobleJump){
                        this.fallspeed = -30;
                        this.hasDobleJump = false;
                    }
                }
            }
            this.fallspeed += Math.pow(1.2,2);
            
            // speed limits
            if(this.speed > this.maxspeed)
                this.speed = this.maxspeed;
            else if(this.speed < -this.maxspeed)
                this.speed = -this.maxspeed;

            if(this.fallspeed > 20)
                this.fallspeed = 20;
            else if(this.fallspeed < -30)
                this.fallspeed = -30;

            //dash input
            if(Input.IsKeyDown(KEY_S) && this.hasDash && this.canDash){
                this.Dash();
            }

            // speed fix
            if(this.speed > 0)
                this.speed = Math.floor(this.speed);
            else
                this.speed = Math.ceil(this.speed);
            if(this.fallspeed > 0)
                this.fallspeed = Math.floor(this.fallspeed);
            else
                this.fallspeed = Math.ceil(this.fallspeed);

        }

    },

    // gestiona las animaciones
    AnimationDirector: function(deltaTime){

        if(this.isKeyLeft)
            this.imageplayer.src = "img/Personaje_Izquierda_.png";
        else 
            this.imageplayer.src = "img/Personaje.png";
        var newframe;
        if(this.timeAnim > 0.1){
            newframe = true;
            this.timeAnim = 0;
        }
        else newframe = false;
        if(newframe){
            if(this.isDead){
                this.posXanim ++;
                if(this.ligth == true){
                    this.ligth = false
                    this.posXanim = 0;
                }
                if(this.posXanim > 4)
                    this.posXanim = 4;
                this.posYanim = 4;
                this.speed= 0;
            }
            else if(this.isInDash){
                this.posXanim = 0;
                this.posYanim = 3;
            }
            else if(this.isInGround){
                if(this.speed > 0.3 ||this.speed < -0.3){
                    if(this.posXanim > 2){
                        this.posXanim =0;
                    }
                    else
                        this.posXanim ++;
                    this.posYanim = 1;
                }
                else{
                    if(this.posXanim > 3){
                        this.posXanim = 0;
                    }
                    else
                        this.posXanim ++;
                    this.posYanim = 0;
                }
            }
            else {
                this.posXanim = 2;
                this.posYanim = 2;
            }
        }
        this.timeAnim += deltaTime;
    },

    // detecta las colisiones con el mapa
    Collision: function(platforms){

        // vertical and horizontal colliders based on the speed
        let horizontalRect = {
            x: this.x + this.speed,
            y:this.y,
            width: this.width,
            height: this.height
        }

        let  verticalRect= {
            x: this.x ,
            y: this.y + this.fallspeed,
            width: this.width,
            height: this.height
        }

        //Collision detected
        for(let i = 0; i < platforms.length; i++){
            if(CollisionDetector(horizontalRect, platforms[i]))
            {
                while(CollisionDetector(horizontalRect, platforms[i]))
                {
                    horizontalRect.x -= this.speed;
                }
                this.x = horizontalRect.x;
                this.speed = 0;
            }
            else if(CollisionDetector(verticalRect, platforms[i]))
            {
                while(CollisionDetector(verticalRect, platforms[i]))
                {
                    verticalRect.y -= this.fallspeed;
                }
                this.y = verticalRect.y;
                if(this.fallspeed > 0)
                    this.SetIsInGround();
                this.fallspeed = 0;
            }
        }
            
    },

    Draw: function (ctx) {
        ctx.drawImage(this.imageplayer,this.posXanim*80,this.posYanim*80 ,80,80,this.x, this.y, 80,80);
    }
}