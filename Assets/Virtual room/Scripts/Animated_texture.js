var frames_count = 0;
var frame_array = new Array (frames_count);
var frame_name : String;
var i = 1;
var time_per_frame=1.0;
var speed; speed = time_per_frame;
var randomize : boolean = false;

function Start(){

	for (var nmbr = 0; nmbr <= frames_count; nmbr++){

		frame_array[nmbr] = Resources.Load (frame_name + nmbr);

	}

} 


function FixedUpdate () {

speed--;

	if(speed <=0){
		speed = time_per_frame;
	}


	if ((i > 0)&&(speed == time_per_frame)&&(randomize==false)){
   		   		
    	this.renderer.material.mainTexture = frame_array[i];
    	i++;
	}


	if ((i > 0)&&(speed == time_per_frame)&&(randomize==true)){
		this.renderer.material.mainTexture = frame_array[(Random.Range(1, frames_count))];
	}


	if (i == frame_array.length){
		i = 1;
	}
	
	


}

