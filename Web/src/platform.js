class Platform
{

    constructor(x,y,width,height,image)
    {
        this.x  = x;
        this.y  = y;
        this.width = width;
        this.height = height;
        this.image = image;
    }
    Start(){
    }

    Update(deltaTime)
    {
    }

    Draw(ctx){
        ctx.drawImage(this.image, this.x, this.y);
    }
}