var Text = {

    text: "null",
    text2: "null",
    visivility: false,
    x: 0,
    y: 0,

    Start: function (canvas) {
        this.x = canvas.width/2;

    },

    Draw: function (ctx) {
        if(this.visivility){
                ctx.fillStyle = '#1D1A29';
                ctx.fillRect(-controller.canvasPositionx,-controller.canvasPositiony,1200,200);
                ctx.fillStyle = "white";
                ctx.font = "25px Verdana";
                ctx.textAlign = 'rigth';
                ctx.fillText(this.text, -controller.canvasPositionx + 20,  -controller.canvasPositiony + 60);
                if(this.text2){
                    ctx.fillText(this.text2, -controller.canvasPositionx + 20,  -controller.canvasPositiony + 110);
                }
        }
    }

}