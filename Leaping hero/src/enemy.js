class Enemy
{

    constructor(x,y,width,height,image,distance)
    {
        this.x  = x;
        this.y  = y;
        this.originalx = x;
        this.width = width;
        this.height = height;
        this.image = image;
        this.posYanim = 0;
        this.posXanim = 0;
        this.speed = 5;
        this.distance = distance;
        this.isDead = false;
        this.timeAnim = 1;
        this.ligth = true;
        this.image2 = new Image(80,80);
        this.image2.src = "img/Slime_azu_Izquierdal.png";
    }
    Start(){
        this.timeAnim = 1;
    }

    Update(deltaTime)
    {
        if(this.x < this.originalx - this.distance/2 )
            this.speed = -this.speed;
        else if(this.x > this.originalx + this.distance/2 )
            this.speed = -this.speed;
        if(CollisionDetector(this, player)){
            if(player.isInDash){
                this.isDead = true;
            }
            else if(this.isDead == false)player.hasDied();
        }

        this.AnimationDirector(deltaTime);

        this.x -= this.speed;
    }

    AnimationDirector (deltaTime){

        //if(this.speed > 0)
        //    this.image.src = "img/Slime_azul.png";
        //else 
        //    this.image.src = "img/Slime_azu_Izquierdal.png";
        var newframe;
        if(this.timeAnim > 0.1){
            newframe = true;
            this.timeAnim = 0;
        }
        else newframe = false;
        if(newframe){
            if(this.isDead){
                if(this.ligth == true){
                    this.ligth = false
                    this.posXanim = 0;
                }
                else if(this.posXanim == 3)
                    controller.enemies.pop(this);
                this.posYanim = 2;
                this.speed= 0;
            }
            else if(player.isDead){
                if(this.ligth){
                    this.ligth = false
                    this.posXanim = 0;
                }
                else if(this.posXanim > 2)
                    this.posXanim = 0;
                this.posYanim = 1;
                this.speed= 0;
            }
            else if(this.posXanim > 2){
                this.posXanim = 0;
            }else this.posYanim = 0;
            
            this.posXanim ++;
        }
        this.timeAnim += deltaTime;
    }

    Draw(ctx){
        if(this.speed > 0)
        ctx.drawImage(this.image,this.posXanim*80,this.posYanim*80 ,80,80,this.x, this.y, 80,80);
        else 
        ctx.drawImage(this.image2,this.posXanim*80,this.posYanim*80 ,80,80,this.x, this.y, 80,80);
    }
}