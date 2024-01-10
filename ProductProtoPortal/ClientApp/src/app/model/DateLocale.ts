export class DateFormat{
  public static toString(date:Date):string{
    return date.toLocaleString('ru-RU',{
      hour12:false,
      month:'2-digit',
      day:'2-digit',
      hour:'2-digit',
      minute:'2-digit'
    })
  }
}
