import { Photo } from './photo';

export interface User {
    id: number;
    username: string;
    knownAs: string;
    age: number;
    gender: string;
    created: Date;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;    // il punto interoggativo ci permette di far divnetare una proprietà facoltativa,
                            // le prop. facolt. DEVONO ESSERE scritte dopo quelle oblligatorie
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
}
