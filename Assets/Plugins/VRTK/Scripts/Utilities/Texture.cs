using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureVR : Texture {

    //fix for the VRTK, since the original Texture class it was using was protected and could not be used
    public TextureVR() {

    }

}
