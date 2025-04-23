function CollisionDetector(objeto1, objeto2){
    if(objeto1.x >= objeto2.x + objeto2.width ||
        objeto1.x + objeto1.width <= objeto2.x ||
        objeto1.y >= objeto2.y + objeto2.height||
        objeto1.y + objeto1.height <= objeto2.y)
        return false;
    else return true;
}