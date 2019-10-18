import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/_models/photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();    // evento per aggiornare in tempo reale la foto profilo
  uploader: FileUploader;
  hasBaseDropZoneOver: false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(private authService: AuthService, private userService: UserService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {    // metodo per uploadare foto
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,    // una volta caricata la foto la togliamo dalla coda di upload
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
  });

 this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };

 this.uploader.onSuccessItem = (item, response, status, headers) => {   // metodo che ci aggiorna in tempo reale le nostre foto aggiunte
   if (response) {
     const res: Photo = JSON.parse(response);
     const photo = {
       id: res.id,
       url: res.url,
       dateAdded: res.dateAdded,
       description: res.description,
       isMain: res.isMain
     };
     this.photos.push(photo);
     if (photo.isMain) {
      this.authService.changeMemberPhoto(photo.url);
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
     }
   }
 };
}

setMainPhoto(photo: Photo) { // metodo per settare una fotoprofilo
  this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
    this.currentMain = this.photos.filter(p => p.isMain === true)[0];
    this.currentMain.isMain = false;
    photo.isMain = true;
    this.authService.changeMemberPhoto(photo.url);
    this.authService.currentUser.photoUrl = photo.url;
    localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
  }, error => {
    this.alertify.error(error);
  });
}

deletePhoto(id: number) {     // metodo per eliminare una foto
  this.alertify.confirm('Sei sicuro di voler eliminare questa foto?', () => {
    this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === id), 1); // slpice elimina elementi da un array
        this.alertify.success('Foto eliminata');
    }, error => {
      this.alertify.error('Errore eliminazione foto');
    });
  });
}
}
