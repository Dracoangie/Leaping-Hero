class Tresure
{

    constructor(x,y,width,height,image)
    {
        this.x  = x;
        this.y  = y;
        this.width = width;
        this.height = height;
        this.image = image;
        this.hasInteracted = false;
        this.image2 = new Image(80,80);
        this.image2.src = "img/Cofre_abierto.png";
    }
    Start(){
    }

    Update(deltaTime)
    {
        if(CollisionDetector(this, player) && Input.IsKeyDown(KEY_DOWN))
        {
            if(this.hasInteracted == false){
                if(player.canDoubleJump == false){
                    player.canDoubleJump = true;
                    this.hasInteracted = true;
                }else player.canDash = true;
                this.hasInteracted = true;
                console.log(this.hasInteracted );
            }
        }
    }

    Draw(ctx){
        if(this.hasInteracted == false)
        ctx.drawImage(this.image, this.x, this.y,this.image.width,this.image.height);
        else
        ctx.drawImage(this.image2, this.x, this.y,this.image.width,this.image2.height);
    }
}