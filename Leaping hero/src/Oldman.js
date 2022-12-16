var OldMan = {

    text: "Oh, God the great leaping hero.",
    text2: "In the deepest part of the cave they have locked our village. We need your help, follow me!",
    visivility: false,
    Image: new Image(80,80),
    x: 1840,
    y: 1280,
    timeWalk: 2,
    numInteraction: 0,

    speed: 4,

    //Triggers
    FirstTrigger : {
        x: 1120,
        y: 880,
        width: 80,
        height: 480
    },

    SecondTrigger : {
        x: 3280,
        y: 2240,
        width: 160,
        height: 80
    },

    Start: function () {
        this.Image.src = "img/oldMan.png";
    },

    Update: function(deltaTime){


        if(CollisionDetector(this.FirstTrigger, player) && this.numInteraction == 0)
        {
            player.canMove = false;
            player.speed = 0;
            player.fallspeed = 10;
            this.timeWalk -= deltaTime;
            this.visivility = true;
            if(this.timeWalk > 0){
                this.x -= this.speed;
            }
            else if(this.timeWalk < -5 && this.timeWalk > -8){
                
                this.Image.src = "img/oldMan_Fliped.png";
                this.x += this.speed;
            }
            else if(this.timeWalk < -8){
                this.numInteraction = 1;
                this.timeWalk = 7;
                this.visivility = false;
                player.canMove = true;
            }

        }
        if(CollisionDetector(this.SecondTrigger, player))
        {
            if(this.visivility == false){this.x = 2880;
                this.y = 142;}
            this.visivility = true;
            player.canMove = false;
            if(this.timeWalk < 5 && this.timeWalk > 1){
                player.imageplayer.src = "img/Personaje_Izquierda_.png";
                this.Image.src = "img/oldMan_Fliped.png";
                if(this.y + 30< 2260)
                    this.y += 30;
            }else if(this.timeWalk <= 1){player.endGame = true;
                player.imageplayer.src = "img/Personaje_Izquierda_.png";}

            if(this.timeWalk > 1){
                this.timeWalk -= deltaTime;
            }
        }
    },

    Draw: function (ctx) {
        if(this.visivility){
            if(this.timeWalk < 0 && this.timeWalk > -5){
                ctx.fillStyle = '#1D1A29';
                ctx.fillRect(-controller.canvasPositionx,-controller.canvasPositiony,1200,200);
                ctx.fillStyle = "white";
                ctx.font = "25px Verdana";
                ctx.textAlign = 'rigth';
                ctx.fillText(this.text, -controller.canvasPositionx + 20,  -controller.canvasPositiony + 60);
                ctx.fillText(this.text2, -controller.canvasPositionx + 20,  -controller.canvasPositiony + 110);
            }
            ctx.drawImage(this.Image,this.x,this.y ,80,80);
        }
    }

}