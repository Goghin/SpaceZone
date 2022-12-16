using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class StarField : MonoBehaviour
{
	public int		MaxStars = 10;
	public float	StarSize = 0.1f;
	public float	StarSizeRange = 0.5f;
	public float	FieldWidth = 20f;
	public float	FieldHeight = 25f;
	public float	ParallaxFactor = 0f;
		
	float 				xOffset;
	float 				yOffset;

	ParticleSystem Particles;
	ParticleSystem.Particle[] Stars;
	

	void Awake ()
	{
		
		Stars = new ParticleSystem.Particle[ MaxStars ];
		Particles = GetComponent<ParticleSystem>();

		Assert.IsNotNull( Particles, "Particle system missing from object!" );

		xOffset = FieldWidth * 0.5f;																										// Offset the coordinates to distribute the spread
		yOffset = FieldHeight * 0.5f;																										// around the object's center
	
		for ( int i=0; i<MaxStars; i++ )
		{
			float randSize = Random.Range( 1f - StarSizeRange, StarSizeRange + 1f );			// Randomize star size within parameters
			float scaledColor =  randSize - StarSizeRange;			// If coloration is desired, color based on size
			Stars[ i ].position = GetRandomInRectangle( FieldWidth, FieldHeight ) + transform.position;
			Stars[ i ].startSize = StarSize * randSize;
			Stars[ i ].startColor = new Color( 1f, scaledColor, scaledColor, 1f );           
		}
		Particles.SetParticles( Stars, Stars.Length );  // Write data to the particle system
	}
	
	void Update ()
	{
		for ( int i=0; i<MaxStars; i++ )
		{
			Vector3 pos = Stars[ i ].position; // + transform.position;
            pos.y -= Time.deltaTime * ParallaxFactor;			
			if ( pos.y < ( -8 ) )
			{
				pos.y += FieldHeight;
                pos.x =  Random.Range( -xOffset, xOffset );               
			}
            Stars[ i ].position = pos;           
		}
		Particles.SetParticles( Stars, Stars.Length );	
	}

	// GetRandomInRectangle
	//----------------------------------------------------------
	// Get a random value within a certain rectangle area
	//
	Vector3 GetRandomInRectangle ( float width, float height )
	{
		float x = Random.Range( 0, width );
		float y = Random.Range( 0, height );
		return new Vector3 ( x - xOffset , y - yOffset, 0 );
	}
}
