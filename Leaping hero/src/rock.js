class Rock{

    constructor(x,y,width,height,image,iD)
    {
        this.x  = x;
        this.y  = y;
        this.width = width;
        this.height = height;
        this.image = image;
        this.iD = iD;
    }

    Update(deltaTime)
    {
        if(CollisionDetector(this, player)){
            //console.log("yess");
            Text.visivility = true;
            switch (this.iD){
                case 0:
                    Text.text = "'Jump is not enough.";
                    Text.text2 = "Take the next treasure, you will be able to dash with the S and kill some slimes if you want.'";
                    controller.ActualID = 0;
                    break;
                case 1:
                    Text.text = "'For the future generations:";
                    Text.text2 = "You can move around the map with the arrow keys, you can also jump with them.'";
                    controller.ActualID = 1;
                    break;
                case 2:
                    Text.text = "'In this world you could find a new jump.";
                    Text.text2 = "But to be able to open it you will have to use the down arrow.'";
                    controller.ActualID = 2;
                    break;
                case 3:
                    Text.text = "'my last piece of advice";
                    Text.text2 = "do NOT trust the old man'";
                    controller.ActualID = 3;
                    break;
                default:
                    break;
            }
        }else if(controller.ActualID == this.iD && Text.visivility){Text.visivility = false;
            controller.ActualID = -1;
        }
    }
}