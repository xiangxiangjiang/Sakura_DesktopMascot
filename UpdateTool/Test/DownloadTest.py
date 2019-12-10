import requests
import progressbar
import requests.packages.urllib3

requests.packages.urllib3.disable_warnings()

url = "http://imgsrc.baidu.com/forum/pic/item/4f2839d2fd1f4134b6073b182a1f95cad1c85e5f.jpg"

headers = {"Host": "github.com",
                   "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36"}

response = requests.request("GET", url, stream=True,
                            data=None, headers=headers)

save_path = "1.jpg"

# print(response.headers)
# total_length = int(response.headers.get("Content-Length"))
with open(save_path, 'wb') as f:
    widgets = ['Processed: ', progressbar.Counter(), ' lines (', progressbar.Timer(), ')']
    pbar = progressbar.ProgressBar(widgets=widgets)
    for chunk in pbar((i for i in response.iter_content(chunk_size=10))):
        if chunk:
            f.write(chunk)
            f.flush()

    # widgets = ['Progress: ', progressbar.Percentage(), ' ',
    #            progressbar.Bar(marker='#', left='[', right=']'),
    #            ' ', progressbar.ETA(), ' ', progressbar.FileTransferSpeed()]
    # pbar = progressbar.ProgressBar(widgets=widgets, maxval=108991).start()
    # for chunk in response.iter_content(chunk_size=1):
    #     if chunk:
    #         f.write(chunk)
    #         f.flush()
    #     pbar.update(len(chunk) + 1)
    # pbar.finish()
